using Verse;
using VFECore.Abilities;

namespace RakazielPsycasts.Carnomancer;

public class Ability_Liberation : Ability
{
    public override Gizmo GetGizmo()
    {
        // ReSharper disable once LocalVariableHidesMember
        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(RPDefOf.RP_Liberation);
        if (hediff == null)
        {
            return base.GetGizmo();
        }

        return new Command_Action
        {
            defaultLabel = "RP_CancelLiberation".Translate(),
            defaultDesc = "RP_CancelLiberationDesc".Translate(),
            icon = def.icon,
            action = delegate
            {
                pawn.health.RemoveHediff(hediff);
            }
        };
    }
}
