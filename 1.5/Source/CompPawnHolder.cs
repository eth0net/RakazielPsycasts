using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RakazielPsycasts;

public class CompPawnHolder : ThingComp, IThingHolder
{
    public ThingOwner<Pawn> innerContainer;

    public Pawn HeldPawn
    {
        get => innerContainer.Count > 0 ? innerContainer[0] : null;
    }

    public bool HoldsPawn => HeldPawn is not null;

    public CompPawnHolder()
    {
        // todo: find a way to disable this comp if no pawns are stored - OnContentsChanged?
        innerContainer = new ThingOwner<Pawn>(this, false);
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public virtual bool Accepts(Pawn pawn)
    {
        return innerContainer.CanAcceptAnyOf(pawn);
    }

    public virtual bool TryAcceptPawn(Pawn pawn)
    {
        if (!Accepts(pawn)) return false;

        // if the pawn is owned, try to transfer it
        if (pawn.holdingOwner is { } owner)
        {
            int count = owner.TryTransferToContainer(pawn, innerContainer, pawn.stackCount);
            return count > 0;
        }

        // if the pawn is not owned, try to add it
        return innerContainer.TryAdd(pawn);
    }

    public virtual void EjectContents()
    {
        innerContainer.TryDropAll(parent.PositionHeld, parent.MapHeld, ThingPlaceMode.Near);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
    }

    public override void PostDestroy(DestroyMode mode, Map map)
    {
        base.PostDestroy(mode, map);
        if (innerContainer.Any && mode is DestroyMode.Deconstruct or DestroyMode.KillFinalize)
        {
            if (mode == DestroyMode.KillFinalize)
            {
                foreach (Pawn pawn in innerContainer.InnerListForReading)
                {
                    HealthUtility.DamageUntilDowned(pawn);
                }
            }

            innerContainer.TryDropAll(parent.PositionHeld, map, ThingPlaceMode.Near);
        }

        innerContainer.ClearAndDestroyContents();
    }

    public override void CompTick()
    {
        base.CompTick();
        innerContainer.ThingOwnerTick();
    }

    public override void CompTickRare()
    {
        base.CompTickRare();
        innerContainer.ThingOwnerTickRare();
    }

    public override void CompTickLong()
    {
        base.CompTickLong();
        innerContainer.ThingOwnerTickLong();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (Gizmo gizmo in base.CompGetGizmosExtra())
        {
            yield return gizmo;
        }

        Gizmo gizmo2 = ContainingSelectionUtility.SelectCarriedThingGizmo(parent, HeldPawn);
        if (gizmo2 is not null)
        {
            yield return gizmo2;
        }
    }

    public override string CompInspectStringExtra()
    {
        string text = base.CompInspectStringExtra();

        if (!innerContainer.Any)
        {
            return text;
        }

        if (!text.NullOrEmpty())
        {
            text += "\n";
        }

        return text + ("RPContains".Translate() + ": " + innerContainer.ContentsString);
    }
}

public class CompProperties_PawnHolder : CompProperties
{
    public CompProperties_PawnHolder()
    {
        compClass = typeof(CompPawnHolder);
    }
}
