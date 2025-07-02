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
                            dryad.connections?.ConnectTo(ability.pawn);
                            Hediff_Floramancer hediff = ability.pawn.GetFloramancerHediff();
                            hediff?.dryads.Add(dryad);

                            Map map = corpse.MapHeld;
                            IntVec3 position = corpse.PositionHeld;

                            corpse.DeSpawn();
                            GenSpawn.Spawn(dryad, position, map).Rotation = Rot4.South;
                            SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));

                            if (dryad.GetCompPawnHolder() is not { } holder)
                            {
                                Log.Error(
                                    $"{nameof(AbilityExtension_Reincarnation)}.{nameof(Cast)}: Failed to get CompPawnHolder for dryad {dryad} at position {position} on map {map}."
                                );
                                continue;
                            }

                            if (!holder.TryAcceptThing(corpse.InnerPawn))
                            {
                                Log.Error(
                                    $"{nameof(AbilityExtension_Reincarnation)}.{nameof(Cast)}: Failed to accept corpse {corpse.InnerPawn} into dryad holder at position {position} on map {map}."
                                );
                            }

                            corpse.Destroy();
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
}
