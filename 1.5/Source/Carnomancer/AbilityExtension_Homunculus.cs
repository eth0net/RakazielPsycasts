using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Carnomancer;

public class AbilityExtension_Homunculus : AbilityExtension_AbilityMod
{
    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);
        foreach (GlobalTargetInfo target in targets)
        {
            if (target.Thing is not Pawn pawn)
            {
                continue;
            }

            // if the pawn has no body parts, skip casting
            if (!pawn.health.hediffSet.GetNotMissingParts().Any())
            {
                continue;
            }

            pawn.health.AddHediff(HediffDefOf.MissingBodyPart, pawn.health.hediffSet.GetNotMissingParts().RandomElement());

            Pawn pawnToSpawn;

            // if the player has anomaly dlc, create a fleshbeast
            if (ModsConfig.AnomalyActive)
            {
                PawnKindDef kind = FleshbeastUtility.AllFleshbeasts.RandomElement();
                pawnToSpawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, pawn.Faction, fixedBiologicalAge: 0f, fixedChronologicalAge: 0f));
                CompInspectStringEmergence compInspectStringEmergence = pawnToSpawn.TryGetComp<CompInspectStringEmergence>();
                if (compInspectStringEmergence != null)
                {
                    compInspectStringEmergence.sourcePawn = pawn;
                }
            }
            // if the player has biotech dlc, create a baby
            else if (ModsConfig.BiotechActive)
            {
                pawnToSpawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawn.kindDef, pawn.Faction, developmentalStages: DevelopmentalStage.Newborn));
            }
            // if the player has neither dlc, create a limbless pawn
            else
            {
                pawnToSpawn = PawnGenerator.GeneratePawn(pawn.kindDef, pawn.Faction);
                foreach (BodyPartRecord part in pawnToSpawn.health.hediffSet.GetNotMissingParts())
                {
                    pawnToSpawn.health.AddHediff(HediffDefOf.MissingBodyPart, part);
                }
            }

            if (pawnToSpawn != null)
            {
                GenSpawn.Spawn(pawnToSpawn, pawn.Position, pawn.Map);
            }
        }
    }
}
