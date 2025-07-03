using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Floramancer;

public class AbilityExtension_Reincarnation : AbilityExtension_AbilityMod
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
                            if (target.Thing is not Corpse corpse)
                            {
                                Log.Error(
                                    $"{nameof(AbilityExtension_Reincarnation)}.{nameof(Cast)}: Target {target} is not a corpse."
                                );
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

                            if (dryad.GetComp<CompCorpseHolder>() is not { } corpseHolder)
                            {
                                Log.Error($"{nameof(AbilityExtension_Reincarnation)}.{nameof(Cast)}: Failed to get CompCorpseHolder for {dryad}.");
                                continue;
                            }

                            IntVec3 position = corpse.Position;
                            Map map = corpse.Map;
                            corpse.DeSpawn();

                            if (!corpseHolder.TryAcceptCorpse(corpse))
                            {
                                Log.Error($"{nameof(AbilityExtension_Reincarnation)}.{nameof(Cast)}: Failed to accept corpse {corpse} into DryadReincarnation.");
                                continue;
                            }

                            floramancerHediff.dryads.Add(dryad);
                            dryad.connections?.ConnectTo(ability.pawn);
                            dryad.health.AddHediff(RPDefOf.RP_DryadReincarnation);

                            GenSpawn.Spawn(dryad, position, map).Rotation = Rot4.South;
                            SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
                        }
                    },
                    dryadCaste,
                    extraPartWidth: 29f,
                    extraPartOnGUI: rect => Widgets.InfoCardButton(rect.x + 5, rect.y + (rect.height - 24f) / 2, dryadCaste)
                );
            }
        ).ToList();

        if (options.Count == 0)
        {
            Messages.Message("No valid dryad modes available for reincarnation.", MessageTypeDefOf.RejectInput, false);
            return;
        }

        if (options.Any())
        {
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }

    public override bool ValidateTarget(LocalTargetInfo target, Ability ability, bool throwMessages = false)
    {
        if (target.Thing is not Corpse corpse)
        {
            if (throwMessages)
            {
                Messages.Message("Target must be a valid corpse.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        if (corpse.InnerPawn?.RaceProps is not { Humanlike: true })
        {
            if (throwMessages)
            {
                Messages.Message("Target corpse must be a humanlike pawn.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        return base.ValidateTarget(target, ability, throwMessages);
    }
}
