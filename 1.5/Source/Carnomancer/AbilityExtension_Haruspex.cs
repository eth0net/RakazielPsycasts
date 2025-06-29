using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Carnomancer;

public class AbilityExtension_Haruspex : AbilityExtension_AbilityMod
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

            // if at least 1 skill has no passion, make random skill with no passion have a major passion
            List<SkillRecord> skills = pawn.skills.skills.Where(skill => skill.passion == Passion.None).ToList();
            if (skills.Any())
            {
                skills.RandomElement().passion = Passion.Major;
                return;
            }

            // else make up to 2 random skills with minor passions to have major passions
            skills = pawn.skills.skills.Where(skill => skill.passion == Passion.Minor).ToList();
            int count = Math.Min(skills.Count, 2);
            for (int i = 0; i < count; i++)
            {
                skills.RandomElement().passion = Passion.Major;
            }
        }
    }
}
