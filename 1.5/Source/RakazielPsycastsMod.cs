using Verse;

namespace RakazielPsycasts;

/// <summary>
///     RakazielPsycasts class to load up the mod and initialise everything.
/// </summary>
public class RakazielPsycasts : Mod
{
    /// <summary>
    ///     RakazielPsycasts constructor to load the mod and settings.
    ///     Also applies patches using harmony.
    /// </summary>
    public RakazielPsycasts(ModContentPack content) : base(content)
    {
    }
}
