using Verse;

namespace RakazielPsycasts.Floramancer;

public class HediffCompProperties_Floramancer : HediffCompProperties
{
    public int baseComaDuration = 5000;

    public int comaDurationPerDryad = 2500;

    public int dryadComaDuration = 60000;

    public int initialDryadBandwidth = 1;

    public HediffCompProperties_Floramancer()
    {
        compClass = typeof(HediffComp_Floramancer);
    }
}
