using RimWorld;
using Verse;
using VFECore.Abilities;

namespace RakazielPsycasts.Floramancer;

public static class FloramancerUtils
{
    public static Hediff_Floramancer GetFloramancerHediff(this Pawn pawn)
    {
        if (pawn?.health?.hediffSet?.GetBrain() is not { } brain)
        {
            Log.Error($"{nameof(FloramancerUtils)}.{nameof(GetFloramancerHediff)}: failed to get brain for pawn {pawn}.");
            return null;
        }

        if (pawn.health.GetOrAddHediff(RPDefOf.RP_Floramancer, brain) is not Hediff_Floramancer hediff)
        {
            Log.Error($"{nameof(FloramancerUtils)}.{nameof(GetFloramancerHediff)}: failed to get or add Hediff_Floramancer for pawn {pawn}.");
            return null;
        }

        return hediff;
    }

    public static bool IsInTreePawnHolder(this Pawn pawn)
    {
        return pawn?.holdingOwner?.Owner is CompPawnHolder { parent: Plant p } && p.def.plant.IsTree;
    }
}
