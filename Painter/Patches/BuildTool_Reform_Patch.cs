using ExcelDsp.Painter.Tool;
using HarmonyLib;

namespace ExcelDsp.Painter.Patches;

/// <summary>Patch for <see cref="BuildTool_Reform"/></summary>
[HarmonyPatch(typeof(BuildTool_Reform))]
internal static class BuildTool_Reform_Patch
{
    /// <summary>Before each update when foundation cursor is active</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform.ReformAction))]
    public static bool ReformAction_Prefix(BuildTool_Reform reformTool)
    {
        if(!FoundationDrawer.IsEnabled)
            return true;

        FoundationDrawer.Update(reformTool);
        return false;
    }
}
