using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RakazielPsycasts;

public class CompCorpseHolder : ThingComp, IThingHolder
{
    public ThingOwner<Corpse> innerContainer;

    public Corpse HeldCorpse => innerContainer.Any ? innerContainer[0] : null;

    public bool HoldsCorpse => HeldCorpse is not null;

    public CompCorpseHolder()
    {
        // todo: find a way to disable this comp if no corpse in stored - OnContentsChanged?
        innerContainer = new ThingOwner<Corpse>(this, true);
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, innerContainer);
    }

    public ThingOwner GetDirectlyHeldThings() => innerContainer;

    public bool Accepts(Corpse corpse) => innerContainer.CanAcceptAnyOf(corpse);

    public bool TryAcceptCorpse(Corpse corpse)
    {
        if (!Accepts(corpse)) return false;

        // if the corpse is owned, try to transfer it
        if (corpse.holdingOwner is { } owner)
        {
            int count = owner.TryTransferToContainer(corpse, innerContainer, corpse.stackCount);
            return count > 0;
        }

        // if the corpse is not owned, try to add it
        return innerContainer.TryAdd(corpse);
    }

    public void EjectContents()
    {
        innerContainer.TryDropAll(parent.PositionHeld, parent.MapHeld, ThingPlaceMode.Near);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        innerContainer.ClearAndDestroyContents();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (Gizmo gizmo in base.CompGetGizmosExtra())
        {
            yield return gizmo;
        }

        Gizmo gizmo2 = ContainingSelectionUtility.SelectCarriedThingGizmo(parent, HeldCorpse);
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

        return text + ("RP_Reincarnating".Translate() + ": " + innerContainer.ContentsString);
    }
}

public class CompProperties_CorpseHolder : CompProperties
{
    public CompProperties_CorpseHolder()
    {
        compClass = typeof(CompCorpseHolder);
    }
}
