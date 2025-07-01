using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Floramancer;

public class AbilityExtension_AwakenTree : AbilityExtension_AbilityMod
{
    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);

        if (targets == null || targets.Length == 0)
        {
            return;
        }

        foreach (GlobalTargetInfo target in targets)
        {
            switch (target.Thing)
            {
                case Pawn pawn when pawn.GetTreeDefForPhytokin() is { } treeDef:
                    // Transform Phytokin type back into the special tree
                    pawn.TransformPawnToTree(treeDef);
                    break;
                case Plant plant when plant.def.GetPhytokinKindForTree() is { } phytokinKind:
                    // Transform special tree into the matching Phytokin type
                    plant.TransformTreeToPawn(phytokinKind);
                    break;
                default:
                    // If the target is neither a valid Pawn with a tree affinity nor a Plant with a Phytokin kind, do nothing
                    Log.Warning($"AwakenTree ability was cast on an invalid target: {target.Thing} of type {target.Thing?.GetType()}");
                    break;
            }
        }
    }

    public override bool Valid(GlobalTargetInfo[] targets, Ability ability, bool throwMessages = false)
    {
        if (targets == null || targets.Length == 0)
        {
            if (throwMessages)
            {
                Messages.Message("No targets selected.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        foreach (GlobalTargetInfo target in targets)
        {
            if (!target.IsValid)
            {
                if (throwMessages)
                {
                    Messages.Message("Invalid target selected.", MessageTypeDefOf.RejectInput, false);
                }

                return false;
            }

            switch (target.Thing)
            {
                case Pawn pawn when pawn.GetTreeDefForPhytokin() is null:
                    {
                        if (throwMessages)
                        {
                            Messages.Message("Selected pawn does not have a valid tree affinity.", MessageTypeDefOf.RejectInput, false);
                        }

                        return false;
                    }
                case Plant plant when plant.def.GetPhytokinKindForTree() == null:
                    {
                        if (throwMessages)
                        {
                            Messages.Message("Selected plant does not have a valid Phytokin kind.", MessageTypeDefOf.RejectInput, false);
                        }

                        return false;
                    }
            }
        }

        return base.Valid(targets, ability, throwMessages);
    }
}
