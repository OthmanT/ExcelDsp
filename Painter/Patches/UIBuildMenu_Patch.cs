using HarmonyLib;

namespace ExcellentDsp.Painter.Patches;

/// <summary>Patch for <see cref="UIBuildMenu"/></summary>
[HarmonyPatch(typeof(UIBuildMenu))]
internal static class UIBuildMenu_Patch
{
    /// <summary>Before opening the "Environment Modification (9)" toolbar</summary>
    [HarmonyPrefix, HarmonyPatch(nameof(UIBuildMenu.OnCategoryButtonClick))]
    public static void OnCategoryButtonClick_Prefix()
        => UIRealtimeTip.Popup(nameof(OnCategoryButtonClick_Prefix));

}
