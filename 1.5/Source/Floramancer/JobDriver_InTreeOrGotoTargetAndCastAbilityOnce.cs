using System.Collections.Generic;
using Verse.AI;
using VFECore.Abilities;

namespace RakazielPsycasts.Floramancer;

public class JobDriver_InTreeOrGotoTargetAndCastAbilityOnce : JobDriver_GotoTargetAndCastAbilityOnce
{
    protected override IEnumerable<Toil> MakeNewToils()
    {
        if (pawn.IsInTreePawnHolder())
        {
            yield return Toils_General.Do(() => CompAbilities.currentlyCasting.Cast(CompAbilities.currentlyCastingTargets));
            yield break;
        }

        foreach (Toil toil in base.MakeNewToils())
            yield return toil;
    }
}
