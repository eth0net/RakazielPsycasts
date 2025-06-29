using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Floramancer;

public class AbilityExtension_TransformItem : AbilityExtension_AbilityMod
{
    [UsedImplicitly]
    public ThingDef fromDef;

    public float multiplier = 1f;

    [UsedImplicitly]
    public ThingDef toDef;

    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);

        if (targets == null || targets.Length == 0)
        {
            return;
        }

        foreach (GlobalTargetInfo target in targets)
        {
            if (target.Thing is not ThingWithComps thing)
            {
                continue;
            }

            if (thing.def != fromDef)
            {
                continue;
            }

            Thing newThing = ThingMaker.MakeThing(toDef);
            newThing.stackCount = Mathf.RoundToInt(thing.stackCount * multiplier);
            newThing.HitPoints = thing.HitPoints;

            Map map = target.Thing.MapHeld;
            IntVec3 cell = target.Thing.PositionHeld;

            thing.Destroy();
            GenSpawn.Spawn(newThing, cell, map);
        }
    }

    public override IEnumerable<string> ConfigErrors()
    {
        if (fromDef == null)
        {
            yield return "fromDef is not set.";
        }

        if (toDef == null)
        {
            yield return "toDef is not set.";
        }
    }
}
