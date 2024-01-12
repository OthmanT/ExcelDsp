using BepInEx.Logging;

namespace ExcelDsp.Painter;

/// <summary>Shared data for the entire assembly</summary>
public static partial class Shared
{
    /// <summary>Shared <see cref="ManualLogSource"/></summary>
    public static ManualLogSource? Logger { get; set; }
}
