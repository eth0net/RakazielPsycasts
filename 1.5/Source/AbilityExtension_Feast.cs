using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts;

public class AbilityExtension_Feast : AbilityExtension_AbilityMod
{
    public float meatModifier = 10.0f;

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

            // spawn meat, quantity depends on the pawn's body size
            Thing meat = ThingMaker.MakeThing(ThingDefOf.Meat_Human);
            meat.stackCount = (int) (pawn.BodySize * meatModifier);
            GenPlace.TryPlaceThing(meat, pawn.Position, pawn.Map, ThingPlaceMode.Near);
        }
    }
}
