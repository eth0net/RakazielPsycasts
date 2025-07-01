using System.Collections.Generic;
using HarmonyLib;
using VanillaIdeologyExpanded_Dryads;
using Verse;

namespace RakazielPsycasts.Floramancer;

[HarmonyPatch(typeof(CompPawnMerge), nameof(CompPawnMerge.SetDryadAwakenPod))]
public class Harmony_CompPawnMerge_SetDryadAwakenPod
{
    public static void Postfix(List<Pawn> pawns)
    {
        if (pawns == null || pawns.Count == 0)
        {
            return;
        }

        foreach (Pawn dryad in pawns)
        {
            foreach (Thing connectedThing in dryad.connections.ConnectedThings)
            {
                if (connectedThing is not Pawn connectedPawn)
                {
                    continue;
                }

                connectedPawn.GetFloramancerHediff()?.dryads.Remove(dryad);
            }
        }
    }
}
