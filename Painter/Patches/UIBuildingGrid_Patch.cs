using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ExcelDsp.Painter.Patches;

/// <summary>Patch for <see cref="UIBuildingGrid"/></summary>
[HarmonyPatch(typeof(UIBuildingGrid))]
internal class UIBuildingGrid_Patch
{
    /// <summary>Before each update action when foundation cursor is active</summary>
    [HarmonyTranspiler, HarmonyPatch("Update")]
    public static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // Patch to use the full array size, instead of subset of the array corresponding to the brush size;
        // this means we'll overflow the subset of the array set by BuildTool_Reform,
        // so BuildTool_Reform also needs to clear the unused values in the array to -1,
        // since UIBuildingGrid already ignores negative values

        FieldInfo brushSize = AccessTools.Field(typeof(BuildTool_Reform), nameof(BuildTool_Reform.brushSize));
        FieldInfo cursorIndices = AccessTools.Field(typeof(BuildTool_Reform), nameof(BuildTool_Reform.cursorIndices));

        // Old code: "int length = reformTool.brushSize * reformTool.brushSize"
        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == brushSize),
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == brushSize),
                new CodeMatch(OpCodes.Mul)
            );

        if(!matcher.IsValid)
            throw new InvalidOperationException("Failed to find patch target");

        // New code: "int length = reformTool.cursorIndices.Length"
        return matcher
            .RemoveInstructions(4)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldfld, cursorIndices),
                new CodeInstruction(OpCodes.Ldlen)
            )
            .InstructionEnumeration();
    }
}
