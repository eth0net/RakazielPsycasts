using System.Collections.Generic;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts;

public class AbilityExtension_FleshBond : AbilityExtension_AbilityMod
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

            // tend targets' wounds and caster gains all wounds from target, already tended
            List<Hediff_Injury> injuries = [];
            pawn.health.hediffSet.GetHediffs(ref injuries);
            foreach (Hediff_Injury injury in injuries)
            {
                injury.Tended(1f, 1f);
                Hediff_Injury casterInjury = (Hediff_Injury) HediffMaker.MakeHediff(injury.def, ability.pawn);
                casterInjury.Severity = injury.Severity;
                ability.pawn.health.AddHediff(casterInjury);
            }
        }
    }
}
