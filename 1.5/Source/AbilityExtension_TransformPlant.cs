using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts;

public class AbilityExtension_TransformPlant : AbilityExtension_AbilityMod
{
    // If true, transforms the plant (crop or tree) into a tree; otherwise, transforms it into a crop.
    public bool transformToTree = false;

    // If true, allows wild plants to be produced; otherwise, only cultivated plants can be produced.
    public bool allowWildPlants = true;

    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);

        if (targets == null || targets.Length == 0) return;

        List<FloatMenuOption> list = [];
        foreach (ThingDef plantDef in ValidPlants(targets))
        {
            string label = plantDef.LabelCap;
            list.Add(
                new FloatMenuOption(
                    label, delegate
                    {
                        foreach (GlobalTargetInfo target in targets)
                        {
                            Plant targetPlant = (Plant) target.Thing;
                            Plant plant = (Plant) ThingMaker.MakeThing(plantDef);

                            plant.Age = targetPlant.Age;
                            plant.Growth = targetPlant.Growth;
                            plant.sown = targetPlant.sown;

                            Map map = targetPlant.MapHeld;
                            IntVec3 cell = targetPlant.PositionHeld;

                            targetPlant.Destroy();
                            GenSpawn.Spawn(plant, cell, map);
                        }
                    }, plantDef, null, forceBasicStyle: false, MenuOptionPriority.Default, null, null, 29f,
                    rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, plantDef)
                )
            );
        }

        if (list.Count == 0)
        {
            Messages.Message("No valid plants can grow at the selected locations.", MessageTypeDefOf.RejectInput, false);
            return;
        }

        if (list.Any()) Find.WindowStack.Add(new FloatMenu(list));
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

        // Check if there are any valid plant defs for all targets
        if (!ValidPlants(targets).Any())
        {
            if (throwMessages)
            {
                Messages.Message("No valid plants can grow at the selected locations.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        return base.Valid(targets, ability, throwMessages);
    }

    public IEnumerable<ThingDef> ValidPlants(GlobalTargetInfo[] targets)
    {
        // todo: figure out why some trees aren't appearing in the list
        return DefDatabase<ThingDef>.AllDefs
            .Where(def =>
                def.category == ThingCategory.Plant &&
                def.plant.IsTree == transformToTree &&
                targets.All(target =>
                    CanEverPlantAt(def, target.Cell, target.Map) &&
                    IsPlantAvailable(def, target.Map)
                )
            );
    }

    public static bool CanEverPlantAt(
        ThingDef plantDef,
        IntVec3 c,
        Map map
    )
    {
        if (plantDef.category != ThingCategory.Plant || !c.InBounds(map))
            return false;

        if (map.fertilityGrid.FertilityAt(c) < (double) plantDef.plant.fertilityMin)
            return false;

        if (plantDef.plant.pollution == Pollution.CleanOnly && c.IsPolluted(map) ||
            plantDef.plant.pollution == Pollution.PollutedOnly && !c.IsPolluted(map))
        {
            return false;
        }

        if (plantDef.passability != Traversability.Impassable) return true;

        foreach (IntVec3 adjCell in GenAdj.CardinalDirections.Select(dir => c + dir).Where(adjCell => adjCell.InBounds(map)))
        {
            Building edifice = adjCell.GetEdifice(map);
            if (edifice?.def.IsDoor != true) continue;
            return false;
        }

        return true;
    }

    public bool IsPlantAvailable(ThingDef plantDef, Map map)
    {
        List<ResearchProjectDef> researchPrerequisites = plantDef.plant.sowResearchPrerequisites;
        if (researchPrerequisites == null)
            return true;
        if (Enumerable.Any(researchPrerequisites, project => !project.IsFinished))
            return false;
        return !plantDef.plant.mustBeWildToSow || (plantDef.plant.mustBeWildToSow && allowWildPlants) || map.Biome.AllWildPlants.Contains(plantDef);
    }
}
