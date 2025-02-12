using System.Linq;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;

namespace RakazielPsycasts;

public class AbilityExtension_Purge : AbilityExtension_AbilityMod
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

            if (!pawn.health.hediffSet.GetNotMissingParts().Any())
            {
                continue;
            }

            pawn.health.AddHediff(HediffDefOf.MissingBodyPart, pawn.health.hediffSet.GetNotMissingParts().RandomElement());

            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.def.isInfection)
                {
                    pawn.health.RemoveHediff(hediff);
                }
            }
        }
    }

    public override bool Valid(GlobalTargetInfo[] targets, Ability ability, bool throwMessages = false)
    {
        // if we have a target with no body part return false
        foreach (GlobalTargetInfo target in targets)
        {
            if (target.Thing is not Pawn pawn)
            {
                continue;
            }

            if (!pawn.health.hediffSet.GetNotMissingParts().Any())
            {
                return false;
            }
        }

        return base.Valid(targets, ability, throwMessages);
    }
}
