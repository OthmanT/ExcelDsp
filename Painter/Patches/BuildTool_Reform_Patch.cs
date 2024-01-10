using HarmonyLib;

namespace ExcellentDsp.Painter.Patches;

/// <summary>Patch for <see cref="BuildTool_Reform"/></summary>
[HarmonyPatch(typeof(BuildTool_Reform))]
internal static class BuildTool_Reform_Patch
{
    /// <summary>Before each update when foundation cursor is active</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform.ReformAction))]
    public static void ReformAction_Prefix()
        => UIRealtimeTip.Popup(nameof(ReformAction_Prefix));
}
