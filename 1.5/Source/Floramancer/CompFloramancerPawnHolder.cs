using System.Collections.Generic;
using RimWorld;
using Verse;
using VFECore.Abilities;

namespace RakazielPsycasts.Floramancer;

public class CompFloramancerPawnHolder : CompPawnHolder
{
    public override void PostDestroy(DestroyMode mode, Map map)
    {
        base.PostDestroy(mode, map);

        foreach (Pawn pawn in innerContainer.InnerListForReading)
        {
            if (!HasOneWithTheForest(pawn)) continue;

            Plant tree = FindAvailableTree(map);
            CompFloramancerPawnHolder holder = tree?.GetComp<CompFloramancerPawnHolder>();
            if (holder == null || !holder.TryAcceptPawn(pawn))
            {
                // Fallback: drop pawn at the destroyed tree's position
                GenSpawn.Spawn(pawn, parent.PositionHeld, map);
            }
        }
    }

    public static bool HasOneWithTheForest(Pawn pawn)
    {
        CompAbilities comp = pawn.GetComp<CompAbilities>();
        return comp != null && comp.HasAbility(RPDefOf.RP_OneWithTheForest);
    }

    public static Plant FindAvailableTree(Map map)
    {
        foreach (Thing thing in map.listerThings.ThingsInGroup(ThingRequestGroup.Plant))
        {
            if (thing is not Plant plant || !plant.def.plant.IsTree) continue;
            if (plant.GetComp<CompFloramancerPawnHolder>() is { HoldsPawn: false }) return plant;
        }

        return null;
    }
}

public class CompProperties_FloramancerPawnHolder : CompProperties_PawnHolder
{
    public CompProperties_FloramancerPawnHolder()
    {
        compClass = typeof(CompFloramancerPawnHolder);
    }
}
