using RimWorld.Planet;
using VFECore.Abilities;

namespace RakazielPsycasts.Floramancer;

public class AbilityExtension_BranchingMind : AbilityExtension_AbilityMod
{
    public int bandwidthIncrease = 1;

    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);
        if (ability?.pawn?.GetFloramancerHediff() is not { } floramancer)
        {
            Log.Error($"{nameof(AbilityExtension_BranchingMind)}.{nameof(Cast)}: failed to get Hediff_Floramancer for pawn {ability.pawn}.");
            return;
        }

        floramancer.ApplyPawnComa();
        floramancer.ApplyDryadsComa();
        floramancer.bandwidth += bandwidthIncrease;
    }
}
