using ExcelDsp.Painter.Tool;
using HarmonyLib;

namespace ExcelDsp.Painter.Patches;

/// <summary>Patch for <see cref="BuildTool_Reform"/></summary>
[HarmonyPatch(typeof(BuildTool_Reform))]
internal static class BuildTool_Reform_Patch
{
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform.UpdateRaycast))]
    public static bool UpdateRaycast_Prefix(BuildTool_Reform __instance)
    {
        // Clear indices to -1 (invalid) instead of 0 (index of the origin point)
        for(int i = 0; i < __instance.cursorIndices.Length; i++)
            __instance.cursorIndices[i] = -1;

        if(!FoundationDrawer.IsEnabled)
            return true;

        FoundationDrawer.UpdateRaycast(__instance);
        return false;
    }

    /// <summary>Before each update action when foundation cursor is active</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform.ReformAction))]
    public static bool ReformAction_Prefix(BuildTool_Reform __instance)
    {
        if(!FoundationDrawer.IsEnabled)
            return true;

        FoundationDrawer.UpdateAction(__instance);
        return false;
    }

    /// <summary>Before exiting the foundation build mode</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(BuildTool_Reform._OnClose))]
    public static void OnClose_Prefix(BuildTool_Reform __instance)
        => FoundationDrawer.Reset(__instance);
}
