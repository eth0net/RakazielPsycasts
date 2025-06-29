using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace RakazielPsycasts.Carnomancer;

public class AbilityExtension_Bloodletting : AbilityExtension_AbilityMod
{
    public float damage = 0.5f;

    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);
        ability.pawn.psychicEntropy.RemoveAllEntropy();
        float calculatedDamage = damage * ability.pawn.health.summaryHealth.SummaryHealthPercent;
        DamageInfo damageInfo = new(DamageDefOf.Psychic, calculatedDamage);
        ability.pawn.TakeDamage(damageInfo);
    }

    public override bool IsEnabledForPawn(Ability ability, out string reason)
    {
        // ReSharper disable once InvertIf
        if (ability.pawn.psychicEntropy.CurrentPsyfocus <= 0f)
        {
            reason = "RP_NotEnoughEntropy".Translate();
            return false;
        }

        return base.IsEnabledForPawn(ability, out reason);
    }
}
