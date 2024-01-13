using ExcelDsp.Painter.Tool;
using HarmonyLib;

namespace ExcelDsp.Painter.Patches;

/// <summary>Patch for <see cref="BuildTool_Reform"/></summary>
[HarmonyPatch(typeof(BuildTool_Reform))]
internal static class BuildTool_Reform_Patch
{
    /// <summary>Before each update action when foundation cursor is active</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform.ReformAction))]
    public static bool ReformAction_Prefix(BuildTool_Reform __instance)
    {
        if(!FoundationDrawer.IsEnabled)
            return true;

        FoundationDrawer.Update(__instance);
        return false;
    }

    /// <summary>Before each update when foundation cursor is active</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform._OnTick))]
    public static void OnTick_Prefix(BuildTool_Reform __instance)
    {
        if(FoundationDrawer.IsEnabled)
            __instance.brushSize = 1;
    }

    /// <summary>Before exiting the foundation build mode</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform._OnClose))]
    public static void OnClose_Prefix()
        => FoundationDrawer.Reset();
}
