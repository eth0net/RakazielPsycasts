using HarmonyLib;
using Verse;

namespace RakazielPsycasts;

/// <summary>
///     Patches the game using Harmony on startup.
/// </summary>
[StaticConstructorOnStartup]
public static class HarmonyPatcher
{
    /// <summary>
    ///     Initializes the Harmony patcher.
    /// </summary>
    static HarmonyPatcher()
    {
        new Harmony("RakazielPsycasts").PatchAll();
    }
}
