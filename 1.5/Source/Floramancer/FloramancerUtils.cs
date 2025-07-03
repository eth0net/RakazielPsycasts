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
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_AnimaAffinity)) return RPDefOf.Plant_TreeAnima;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_GauranlenAffinity)) return RPDefOf.Plant_TreeGauranlen;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_PoluxAffinity)) return RPDefOf.Plant_TreePolux;
        return null;
    }

    public static XenotypeDef GetPhytokinKindForTree(this ThingDef treeDef)
    {
        return treeDef switch
        {
            not null when treeDef == RPDefOf.Plant_TreeAnima => RPDefOf.VRE_Animakin,
            not null when treeDef == RPDefOf.Plant_TreeGauranlen => RPDefOf.VRE_Gauranlenkin,
            not null when treeDef == RPDefOf.Plant_TreePolux => RPDefOf.VRE_Poluxkin,
            _ => null
        };
    }

    public static void TransformPawnToTree(this Pawn pawn, ThingDef treeDef)
    {
        Thing tree = ThingMaker.MakeThing(treeDef);
        if (tree.GetCompPawnHolder() is not { } treeHolder)
        {
            Log.Error($"{nameof(FloramancerUtils)}.{nameof(TransformPawnToTree)}: Failed to get CompPawnHolder for tree {tree}.");
            return;
        }

        if (!treeHolder.TryAcceptThing(pawn))
        {
            Log.Error($"{nameof(FloramancerUtils)}.{nameof(TransformPawnToTree)}: Failed to accept pawn {pawn} into tree holder.");
        }

        IntVec3 position = pawn.Position;
        Map map = pawn.Map;
        pawn.DeSpawn();
        GenSpawn.Spawn(tree, position, map);
    }

    public static void TransformTreeToPawn(this Plant tree, XenotypeDef phytokinKind)
    {
        IntVec3 position = tree.Position;
        Map map = tree.Map;

        CompPawnHolder comp = tree.GetCompPawnHolder();

        if (comp.HoldsPawn)
        {
            // Spawn the stored pawn if it exists
            comp.EjectContents();
        }
        else
        {
            // Generate a new pawn otherwise
            Pawn phytokin = PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(
                    kind: Faction.OfPlayer.RandomPawnKind(),
                    faction: Faction.OfPlayer,
                    forceGenerateNewPawn: true,
                    canGeneratePawnRelations: false,
                    colonistRelationChanceFactor: 0,
                    allowFood: false,
                    allowAddictions: false,
                    // fixedBiologicalAge: tree.Age,
                    // fixedChronologicalAge: tree.Age,
                    forceNoIdeo: true,
                    forceNoBackstory: true,
                    forbidAnyTitle: true,
                    forcedXenotype: phytokinKind,
                    dontGiveWeapon: true,
                    forceNoGear: true
                )
            );
            GenSpawn.Spawn(phytokin, position, map);
        }

        tree.Destroy();
    }

    public static CompPawnHolder GetCompPawnHolder(this Thing thing)
    {
        return thing.TryGetComp<CompPawnHolder>();
    }
}
