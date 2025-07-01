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

        List<FloatMenuOption> options = [];
        foreach (GauranlenTreeModeDef mode in DefDatabase<GauranlenTreeModeDef>.AllDefs.ToList())
        {
            string label = mode.LabelCap;
            PawnKindDef dryadKind = mode.pawnKindDef;
            ThingDef dryadCaste = dryadKind.race;
            options.Add(
                new FloatMenuOption(
                    label, delegate
                    {
                        foreach (GlobalTargetInfo target in targets)
                        {
                            if (target.Thing is not Plant plant)
                            {
                                continue;
                            }

                            Pawn dryad = PawnGenerator.GeneratePawn(
                                new PawnGenerationRequest(
                                    kind: dryadKind, faction: Faction.OfPlayer, context: PawnGenerationContext.NonPlayer, tile: -1, forceGenerateNewPawn: false, allowDead: false,
                                    allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, colonistRelationChanceFactor: 1f,
                                    forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false,
                                    certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0f,
                                    biocodeApparelChance: 0f, extraPawnForExtraRelationChance: null, relationWithExtraPawnChanceFactor: 1f, validatorPreGear: null,
                                    validatorPostGear: null, forcedTraits: null, prohibitedTraits: null, minChanceToRedressWorldPawn: null, fixedBiologicalAge: null,
                                    fixedChronologicalAge: null, fixedGender: Gender.Male, fixedLastName: null, fixedBirthName: null, fixedTitle: null, fixedIdeo: null,
                                    forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, forcedXenogenes: null, forcedEndogenes: null,
                                    forcedXenotype: null, forcedCustomXenotype: null, allowedXenotypes: null, forceBaselinerChance: 0f,
                                    developmentalStages: DevelopmentalStage.Newborn
                                )
                            );
                            dryad.connections?.ConnectTo(ability.pawn);
                            Hediff_Floramancer hediff = ability.pawn.GetFloramancerHediff();
                            hediff?.dryads.Add(dryad);

                            Map map = target.Thing.MapHeld;
                            IntVec3 cell = target.Thing.PositionHeld;

                            plant.Destroy();
                            GenSpawn.Spawn(dryad, cell, map).Rotation = Rot4.South;
                            SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
                        }
                    }, dryadCaste, null, forceBasicStyle: false, MenuOptionPriority.Default, null, null, 29f,
                    rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, dryadCaste)
                )
            );
        }

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
