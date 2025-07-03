using RimWorld;
using Verse;

namespace RakazielPsycasts.Floramancer;

public class HediffComp_DryadReincarnation : HediffComp //, IThingHolder
{
    public CompCorpseHolder holder => Pawn.GetComp<CompCorpseHolder>();

    public HediffCompProperties_DryadReincarnation Props => (HediffCompProperties_DryadReincarnation) props;

    public override bool CompShouldRemove => parent.ageTicks >= Props.Ticks;

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();

        if (!CompShouldRemove)
        {
            Log.Error($"{nameof(HediffComp_DryadReincarnation)}.{nameof(CompPostPostRemoved)}: not reincarnating as time not met.");
            holder.innerContainer.ClearAndDestroyContents();
            return;
        }

        if (holder is null)
        {
            Log.Error($"{nameof(HediffComp_DryadReincarnation)}.{nameof(CompPostPostRemoved)}: No holder found for reincarnation.");
            return;
        }

        if (holder.HeldCorpse is not { } corpse)
        {
            Log.Error($"{nameof(HediffComp_DryadReincarnation)}.{nameof(CompPostPostRemoved)}: No corpse found for reincarnation.");
            holder.innerContainer.ClearAndDestroyContents();
            return;
        }

        Pawn innerPawn = corpse.InnerPawn;
        ResurrectionParams parms = new() { dontSpawn = false };
        if (!ResurrectionUtility.TryResurrect(innerPawn, parms))
        {
            Log.Error($"{nameof(HediffComp_DryadReincarnation)}.{nameof(CompPostPostRemoved)}: Failed to resurrect pawn {innerPawn} for corpse {corpse}.");
            holder.innerContainer.ClearAndDestroyContents();
            return;
        }

        string letterText = "RP_ReincarnatedLetter".Translate(innerPawn.LabelShort, innerPawn.Named("PAWN"));
        Find.LetterStack.ReceiveLetter(
            "RP_Reincarnated".Translate() + ": " + innerPawn.LabelShortCap,
            letterText,
            LetterDefOf.PositiveEvent,
            (Thing) innerPawn
        );

        parent.pawn.Destroy();
    }

    public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
    {
        base.Notify_PawnDied(dinfo, culprit);
        holder.innerContainer.ClearAndDestroyContents();
    }
}
