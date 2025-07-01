using System.Collections.Generic;
using RimWorld;
using VanillaPsycastsExpanded;
using Verse;

namespace RakazielPsycasts.Floramancer;

public class Hediff_Floramancer : HediffWithComps
{
    public int bandwidth;

    public List<Pawn> dryads = [];

    public DryadBandwidthGizmo bandwidthGizmo;

    public HediffCompProperties_Floramancer Props => GetComp<HediffComp_Floramancer>().Props;

    public int ComaDuration => Props.baseComaDuration + dryads.Count * Props.comaDurationPerDryad;

    public override IEnumerable<Gizmo> GetGizmos()
    {
        if (!pawn.IsColonistPlayerControlled)
        {
            yield break;
        }

        yield return bandwidthGizmo;
    }

    public override void PostMake()
    {
        base.PostMake();
        bandwidth = Props.initialDryadBandwidth;
        bandwidthGizmo ??= new DryadBandwidthGizmo(this);
    }

    public override void PostRemoved()
    {
        base.PostRemoved();
        if (!IsAlive())
        {
            ClearDryads();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref dryads, "dryads", LookMode.Reference);
        Scribe_Values.Look(ref bandwidth, "bandwidth", 1);
    }

    public void ClearDryads()
    {
        foreach (Pawn dryad in dryads)
        {
            if (dryad is not { Destroyed: false })
            {
                continue;
            }

            dryad.connections?.Notify_ConnectedThingDestroyed(pawn);
            dryad.forceNoDeathNotification = true;
            dryad.Kill(null);
            dryad.forceNoDeathNotification = false;
        }

        dryads.Clear();
    }

    public bool IsAlive()
    {
        if (pawn == null)
        {
            return false;
        }

        if (!pawn.Dead)
        {
            return !pawn.Destroyed;
        }

        return false;
    }

    public void ApplyPawnComa()
    {
        Hediff hediff = pawn.health.AddHediff(VPE_DefOf.PsychicComa);
        hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = ComaDuration;
        // todo: apply motes like puppeteer?
    }

    public void ApplyDryadsComa()
    {
        foreach (Pawn dryad in dryads)
        {
            Hediff hediff = dryad.health.AddHediff(VPE_DefOf.PsychicComa);
            hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = Props.dryadComaDuration;
            // todo: apply motes like puppeteer?
        }
    }
}
