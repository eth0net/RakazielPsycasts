using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts;

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

            // if the player has anomaly dlc, create a fleshbeast
            if (ModLister.HasActiveModWithName("Anomaly"))
            {
                Pawn fleshbeast = PawnGenerator.GeneratePawn(PawnKindDef.Named("Fleshbeast"), pawn.Faction);
                fleshbeast.Name = new NameSingle("Fleshbeast");
            }
            // if the player has biotech dlc, create a limbless baby
            else if (ModLister.HasActiveModWithName("Biotech"))
            {
                Pawn baby = PawnGenerator.GeneratePawn(PawnKindDef.Named("Baby"), pawn.Faction);
                baby.Name = new NameSingle("Baby");
                baby.health.AddHediff(HediffDefOf.MissingBodyPart, baby.health.hediffSet.GetNotMissingParts().RandomElement());
            }
            // if the player has neither dlc, create a limbless pawn
            else
            {
                Pawn limblessPawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("LimblessPawn"), pawn.Faction);
                limblessPawn.Name = new NameSingle("LimblessPawn");
                limblessPawn.health.AddHediff(HediffDefOf.MissingBodyPart, limblessPawn.health.hediffSet.GetNotMissingParts().RandomElement());
            }
        }
    }
}
