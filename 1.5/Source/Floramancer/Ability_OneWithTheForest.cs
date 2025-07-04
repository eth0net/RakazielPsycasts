using RimWorld;
using RimWorld.Planet;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Floramancer;

public class Ability_OneWithTheForest : Ability
{
    public override bool ShowGizmoOnPawn()
    {
        // Show the gizmo if the pawn is merged with a tree
        bool oneWithTheForest = pawn.IsInTreePawnHolder();

        return oneWithTheForest || base.ShowGizmoOnPawn();
    }

    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);

        if (targets == null || targets.Length == 0)
        {
            return;
        }

        foreach (GlobalTargetInfo target in targets)
        {
            if (target.Thing is not Plant plant)
            {
                Log.Error($"{nameof(Ability_OneWithTheForest)}.{nameof(Cast)}: Target {target} is not a plant.");
                continue;
            }

            if (plant.GetComp<CompPawnHolder>() is not { } pawnHolder)
            {
                Log.Error($"{nameof(Ability_OneWithTheForest)}.{nameof(Cast)}: Failed to get CompPawnHolder for plant {plant}.");
                continue;
            }

            IntVec3 pos = plant.Position;
            Map map = plant.Map;
            if (pawn.Spawned) pawn.DeSpawn();

            if (!pawnHolder.TryAcceptPawn(pawn))
            {
                Log.Error($"{nameof(Ability_OneWithTheForest)}.{nameof(Cast)}: Failed to accept pawn {pawn} into plant holder.");
                GenSpawn.Spawn(pawn, pos, map);
            }
        }
    }

    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
    {
        if (target == null)
        {
            if (showMessages)
            {
                Messages.Message("No target selected.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        if (target.Thing is not Plant plant || !plant.def.plant.IsTree)
        {
            if (showMessages)
            {
                Messages.Message("Invalid target selected. Must be a tree.", MessageTypeDefOf.RejectInput, false);
            }

            return false;
        }

        return base.ValidateTarget(target, showMessages);
    }
}
