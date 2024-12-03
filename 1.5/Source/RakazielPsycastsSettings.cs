using Verse;

namespace RakazielPsycasts;

/// <summary>
///     Mod settings for RakazielPsycasts.
/// </summary>
public class RakazielPsycastsSettings : ModSettings
{
    /// <summary>
    ///     An example setting.
    /// </summary>
    public static bool exampleSetting = false;

    /// <summary>
    ///     ExposeData saves and loads the settings.
    /// </summary>
    public override void ExposeData()
    {
        Scribe_Values.Look(ref exampleSetting, "exampleSetting", false);
        base.ExposeData();
    }
}
