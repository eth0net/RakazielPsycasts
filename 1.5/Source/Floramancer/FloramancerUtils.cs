using RimWorld;
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

    public static ThingDef GetTreeDefForPhytokin(this Pawn pawn)
    {
        if (pawn?.genes == null) return null;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_AnimaAffinity)) return RPDefOf.AnimaTree;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_GauranlenAffinity)) return RPDefOf.GauranlenTree;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_PoluxAffinity)) return RPDefOf.PoluxTree;
        return null;
    }

    public static PawnKindDef GetPhytokinKindForTree(this ThingDef treeDef)
    {
        if (treeDef == RPDefOf.AnimaTree) return RPDefOf.VRE_AnimaPhytokin;
        if (treeDef == RPDefOf.GauranlenTree) return RPDefOf.VRE_GauranlenPhytokin;
        if (treeDef == RPDefOf.PoluxTree) return RPDefOf.VRE_PoluxPhytokin;
        return null;
    }

    public static void TransformPawnToTree(this Pawn pawn, ThingDef treeDef)
    {
        IntVec3 position = pawn.Position;
        Map map = pawn.Map;
        pawn.DeSpawn();
        GenSpawn.Spawn(treeDef, position, map);
    }

    public static void TransformTreeToPawn(this Plant plant, PawnKindDef phytokinKind)
    {
        IntVec3 position = plant.Position;
        Map map = plant.Map;
        plant.Destroy();
        Pawn pythokin = PawnGenerator.GeneratePawn(phytokinKind, Faction.OfPlayer);
        GenSpawn.Spawn(pythokin, position, map);
    }
}
