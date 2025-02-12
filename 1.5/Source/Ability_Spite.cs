using System.Collections.Generic;
using RimWorld.Planet;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts;

public class Ability_Spite : Ability
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

            // get a list of injuries on the caster
            List<Hediff> injurires = [];
            pawn.health.hediffSet.GetHediffs(ref injurires, h => h.def.isBad);

            // if the caster has no injuries, skip it
            if (!injurires.Any())
            {
                continue;
            }

            // copy injuries from the caster to the target
            foreach (Hediff injury in injurires)
            {
                targetPawn.health.AddHediff(injury);
            }

            // tend, but don't remove, the injuries on the caster
            foreach (Hediff injury in injurires)
            {
                injury.Tended(9999, 0);
            }

            // add a message
            // Messages.Message("RP_SpiteSuccess".Translate(pawn.LabelShort, targetPawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent);
        }
    }
}
