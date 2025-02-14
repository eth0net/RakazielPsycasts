using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts;

public class Ability_Avarice : Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        foreach (GlobalTargetInfo target in targets)
        {
            if (target.Thing is not Pawn targetPawn)
            {
                continue;
            }

            // get a list of parts the caster is missing that the target has
            List<BodyPartRecord> partsToSteal = targetPawn.health.hediffSet.GetNotMissingParts()
                .Where(part => !pawn.health.hediffSet.GetNotMissingParts().Contains(part)).ToList();

            // if the target has no parts the caster is missing, skip it
            if (!partsToSteal.Any())
            {
                continue;
            }

            // steal a random part from the target
            BodyPartRecord partToSteal = partsToSteal.RandomElement();

            // remove the part from the target
            targetPawn.health.AddHediff(HediffDefOf.MissingBodyPart, partToSteal);

            // add the part to the caster
            pawn.health.RestorePart(partToSteal);

            // add a message
            // Messages.Message("RP_AvariceSuccess".Translate(pawn.LabelShort, targetPawn.LabelShort, partToSteal.Label), pawn, MessageTypeDefOf.PositiveEvent);
        }
    }
}
