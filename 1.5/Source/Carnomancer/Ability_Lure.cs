using RimWorld;
using RimWorld.Planet;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Carnomancer;

public class Ability_Lure : Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        foreach (GlobalTargetInfo target in targets)
        {
            if (target.Thing is not Pawn targetPawn || !targetPawn.AnimalOrWildMan())
            {
                continue;
            }

            bool isManhunter = targetPawn.MentalStateDef == MentalStateDefOf.Manhunter || targetPawn.MentalStateDef == MentalStateDefOf.ManhunterPermanent;
            bool success = Rand.Chance(SuccessChanceOn(targetPawn));

            switch (success)
            {
                case false when !isManhunter:
                    pawn.mindState.mentalStateHandler.TryStartMentalState(
                        MentalStateDefOf.Manhunter, "AnimalManhunterFromTaming".Translate(),
                        true, false, false, null,
                        false, false, true
                    );
                    break;
                case true when isManhunter: pawn.MentalState.RecoverFromState(); break;
                case true: InteractionWorker_RecruitAttempt.DoRecruit(pawn, pawn); break;
            }
        }
    }

    private float SuccessChanceOn(Pawn target)
    {
        return pawn.GetStatValue(StatDefOf.PsychicSensitivity) - target.RaceProps.wildness;
    }
}
