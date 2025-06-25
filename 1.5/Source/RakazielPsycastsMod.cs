using HarmonyLib;
using UnityEngine;
using Verse;

namespace RakazielPsycasts;

/// <summary>
///     RakazielPsycasts class to load up the mod and initialise everything.
/// </summary>
public class RakazielPsycasts : Mod
{
    /// <summary>
    ///     RakazielPsycasts settings reference.
    /// </summary>
    internal static RakazielPsycastsSettings settings;

    /// <summary>
    ///     RakazielPsycasts constructor to load the mod and settings.
    ///     Also applies patches using harmony.
    /// </summary>
    public RakazielPsycasts(ModContentPack content) : base(content)
    {
        settings = GetSettings<RakazielPsycastsSettings>();
    }

    /// <summary>
    ///     DoSettingsWindowContents configures the settings window.
    /// </summary>
    /// <param name="inRect"></param>
    public override void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listing = new();

        listing.Begin(inRect);

        listing.GapLine();
        listing.Label("Example Settings");
        listing.GapLine();

        listing.CheckboxLabeled("Example Setting", ref RakazielPsycastsSettings.exampleSetting, "Example setting description");

        listing.End();

        base.DoSettingsWindowContents(inRect);
    }

    /// <summary>
    ///     SettingsCategory returns the name of the settings category.
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "RakazielPsycasts".Translate();
    }
}
