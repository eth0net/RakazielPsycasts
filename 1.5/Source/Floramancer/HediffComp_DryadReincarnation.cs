using Verse;

namespace RakazielPsycasts.Floramancer;

public class HediffComp_DryadReincarnation : HediffComp
{
    public HediffCompProperties_DryadReincarnation Props => (HediffCompProperties_DryadReincarnation) props;

    public override bool CompShouldRemove => parent.ageTicks >= Props.Ticks;

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();

        CompPawnHolder holder = parent.pawn.GetCompPawnHolder();

        if (!CompShouldRemove)
        {
            Log.Error($"{nameof(HediffComp_DryadReincarnation)}.{nameof(CompPostPostRemoved)}: not reincarnating as time not met.");
            holder.innerContainer.ClearAndDestroyContents();
            return;
        }

        if (!holder.HoldsPawn)
        {
            Log.Error($"{nameof(HediffComp_DryadReincarnation)}.{nameof(CompPostPostRemoved)}: Pawn {parent.pawn} does not hold a pawn for reincarnation.");
            return;
        }

        holder.EjectContents();
        parent.pawn.Destroy();
    }
}
