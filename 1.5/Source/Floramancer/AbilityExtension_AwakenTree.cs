using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Floramancer;

public class AbilityExtension_AwakenTree : AbilityExtension_AbilityMod
{
    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);

        if (targets == null || targets.Length == 0)
        {
            return;
        }

        foreach (GlobalTargetInfo target in targets)
        {
            switch (target.Thing)
            {
                case Pawn pawn when GetTreeDefForPawn(pawn) is { } treeDef:
                    TransformPawnToTree(pawn, treeDef);
                    break;

                case Plant plant when GetPhytokinKindForTree(plant.def) is { } phytokinKind:
                    TransformTreeToPawn(plant, phytokinKind);
                    break;

                default:
                    Log.Warning($"AwakenTree ability was cast on an invalid target: {target.Thing} of type {target.Thing?.GetType()} at {target.Cell}");
                    break;
            }
        }
    }

    public override bool Valid(GlobalTargetInfo[] targets, Ability ability, bool throwMessages = false)
    {
        if (targets == null || targets.Length == 0)
        {
            if (throwMessages)
            {
                Messages.Message("No targets selected.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        foreach (GlobalTargetInfo target in targets)
        {
            if (!target.IsValid)
            {
                if (throwMessages)
                {
                    Messages.Message("Invalid target selected.", MessageTypeDefOf.RejectInput, false);
                }

                return false;
            }

            switch (target.Thing)
            {
                case Pawn pawn when GetTreeDefForPawn(pawn) is null:
                    {
                        if (throwMessages)
                        {
                            Messages.Message("Selected pawn does not have a valid tree affinity.", MessageTypeDefOf.RejectInput, false);
                        }

                        return false;
                    }
                case Plant plant when GetPhytokinKindForTree(plant.def) == null:
                    {
                        if (throwMessages)
                        {
                            Messages.Message("Selected plant does not have a valid Phytokin kind.", MessageTypeDefOf.RejectInput, false);
                        }

                        return false;
                    }
            }
        }

        return base.Valid(targets, ability, throwMessages);
    }

    public static void TransformPawnToTree(Pawn pawn, ThingDef treeDef)
    {
        Thing tree = ThingMaker.MakeThing(treeDef);
        if (tree.TryGetComp<CompPawnHolder>() is not { } treeHolder)
        {
            Log.Error($"{nameof(AbilityExtension_AwakenTree)}.{nameof(TransformPawnToTree)}: Failed to get CompPawnHolder for tree {tree}.");
            return;
        }

        if (!treeHolder.TryAcceptPawn(pawn))
        {
            Log.Error($"{nameof(AbilityExtension_AwakenTree)}.{nameof(TransformPawnToTree)}: Failed to accept pawn {pawn} into tree holder.");
        }

        IntVec3 position = pawn.Position;
        Map map = pawn.Map;
        pawn.DeSpawn();
        GenSpawn.Spawn(tree, position, map);
    }

    public static void TransformTreeToPawn(Plant tree, XenotypeDef phytokinKind)
    {
        CompPawnHolder comp = tree.GetComp<CompPawnHolder>();
        IntVec3 position = tree.Position;
        Map map = tree.Map;

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

    public static ThingDef GetTreeDefForPawn(Pawn pawn)
    {
        if (pawn?.genes == null) return null;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_AnimaAffinity)) return RPDefOf.Plant_TreeAnima;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_GauranlenAffinity)) return RPDefOf.Plant_TreeGauranlen;
        if (pawn.genes.HasActiveGene(RPDefOf.VRE_PoluxAffinity)) return RPDefOf.Plant_TreePolux;
        return null;
    }

    public static XenotypeDef GetPhytokinKindForTree(ThingDef treeDef)
    {
        return treeDef switch
        {
            not null when treeDef == RPDefOf.Plant_TreeAnima => RPDefOf.VRE_Animakin,
            not null when treeDef == RPDefOf.Plant_TreeGauranlen => RPDefOf.VRE_Gauranlenkin,
            not null when treeDef == RPDefOf.Plant_TreePolux => RPDefOf.VRE_Poluxkin,
            _ => null
        };
    }
}
