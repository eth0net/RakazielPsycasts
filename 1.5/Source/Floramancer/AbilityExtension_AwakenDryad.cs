using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Floramancer;

public class AbilityExtension_AwakenDryad : AbilityExtension_AbilityMod
{
    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);

        if (targets == null || targets.Length == 0)
        {
            return;
        }

        List<FloatMenuOption> options = DefDatabase<GauranlenTreeModeDef>.AllDefs.Select(mode =>
            {
                string label = mode.LabelCap;
                PawnKindDef dryadKind = mode.pawnKindDef;
                ThingDef dryadCaste = dryadKind.race;
                return new FloatMenuOption(
                    label, delegate
                    {
                        foreach (GlobalTargetInfo target in targets)
                        {
                            if (target.Thing is not Plant plant)
                            {
                                Log.Error($"{nameof(AbilityExtension_AwakenDryad)}.{nameof(Cast)}: Target {target} is not a plant.");
                                continue;
                            }

                            if (ability.pawn.GetFloramancerHediff() is not { } floramancerHediff)
                            {
                                Log.Error($"{nameof(AbilityExtension_Reincarnation)}.{nameof(Cast)}: Failed to get Hediff_Floramancer for {ability.pawn}.");
                                continue;
                            }

                            Pawn dryad = PawnGenerator.GeneratePawn(
                                new PawnGenerationRequest(
                                    kind: dryadKind,
                                    faction: Faction.OfPlayer,
                                    forceGenerateNewPawn: true,
                                    canGeneratePawnRelations: false,
                                    developmentalStages: DevelopmentalStage.Newborn
                                )
                            );

                            Map map = target.Thing.MapHeld;
                            IntVec3 cell = target.Thing.PositionHeld;
                            plant.Destroy();

                            dryad.connections?.ConnectTo(ability.pawn);
                            floramancerHediff.dryads.Add(dryad);

                            GenSpawn.Spawn(dryad, cell, map).Rotation = Rot4.South;
                            SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
                        }
                    },
                    dryadCaste,
                    extraPartWidth: 29f,
                    extraPartOnGUI: rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, dryadCaste)
                );
            }
        ).ToList();

        if (options.Count == 0)
        {
            Messages.Message("RP_Floramancer_NoDryadModes".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        if (options.Any())
        {
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }

    public override bool Valid(GlobalTargetInfo[] targets, Ability ability, bool throwMessages = false)
    {
        // Check for valid targets
        if (targets == null || targets.Length == 0)
        {
            if (throwMessages)
            {
                Messages.Message("No targets selected.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        if (targets.Any(target => !target.IsValid))
        {
            if (throwMessages)
            {
                Messages.Message("Invalid target selected.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        if (targets.Any(target => target.Thing is not Plant plant || !plant.def.plant.IsTree))
        {
            if (throwMessages)
            {
                Messages.Message("RP_Floramancer_TargetNotTree".Translate(), MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        return base.Valid(targets, ability, throwMessages);
    }
}
