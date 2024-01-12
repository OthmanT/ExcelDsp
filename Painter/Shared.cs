using BepInEx.Logging;

namespace ExcellentDsp.Painter;

/// <summary>Shared data for the entire assembly</summary>
public static partial class Shared
{
    /// <summary>Shared <see cref="ManualLogSource"/></summary>
    public static ManualLogSource? Logger { get; set; }

    /// <summary>Whether the plugin is currently enabled</summary>
    public static bool IsEnabled { get; set; } = false;
}
