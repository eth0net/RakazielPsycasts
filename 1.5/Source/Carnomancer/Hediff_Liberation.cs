using Verse;

namespace RakazielPsycasts.Carnomancer;

public class Hediff_Liberation : HediffWithComps
{
    public override bool ShouldRemove
    {
        get
        {
            // if no missing limbs, return
            if (pawn.health.hediffSet.GetMissingPartsCommonAncestors().Count == 0)
            {
                return true;
            }

            // if psyfocus is full, return
            if (pawn.psychicEntropy.CurrentPsyfocus >= 1f)
            {
                return true;
            }

            return base.ShouldRemove;
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (Find.TickManager.TicksGame % 60 != 0)
        {
            return;
        }

        float offset = 0.01f;
        offset *= pawn.health.hediffSet.GetMissingPartsCommonAncestors().Count;
        pawn.psychicEntropy.OffsetPsyfocusDirectly(offset);
    }
}
