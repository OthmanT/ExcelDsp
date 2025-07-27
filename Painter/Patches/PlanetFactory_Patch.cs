using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace ExcelDsp.Painter.Patches {
    /// <summary>Patch for <see cref="PlanetFactory"/> to expose private fields</summary>
    [HarmonyPatch(typeof(PlanetFactory))]
    public static class PlanetFactory_Patch {
        /// <summary>A public reference to the private tmp_levelChanges dictionary</summary>
        public static Dictionary<int, int>? TmpLevelChanges;

        /// <summary>After the game runs BeginFlattenTerrain, grab a reference to the private dictionary</summary>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlanetFactory.BeginFlattenTerrain))]
        public static void BeginFlattenTerrain_Postfix(PlanetFactory __instance) {
            // Use reflection to get the private field
            var fieldInfo = typeof(PlanetFactory).GetField("tmp_levelChanges", BindingFlags.NonPublic | BindingFlags.Instance);
            // Store the reference in our public static field
            TmpLevelChanges = (Dictionary<int, int>)fieldInfo.GetValue(__instance);
        }
    }
}