using RimWorld;
using Verse;

namespace RakazielPsycasts.Floramancer;

public class HediffCompProperties_DryadReincarnation : HediffCompProperties
{
    public int days = 60;

    public int Ticks => days * GenDate.TicksPerDay;

    public HediffCompProperties_DryadReincarnation()
    {
        compClass = typeof(HediffComp_DryadReincarnation);
    }
}
