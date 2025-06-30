using Verse;

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
}
