using ExcelDsp.Painter.Extensions;
using System;
using UnityEngine;

namespace ExcelDsp.Painter.Tool;

/// <summary>Draws foundation tiles</summary>
internal static class FoundationDrawer
{
    private static Vector3? _start;

    /// <summary>Whether this is currently enabled, overriding the vanilla foundation logic</summary>
    public static bool IsEnabled { get; set; }

    /// <summary>Update tool logic each frame (when enabled and active)</summary>
    /// <param name="reformTool">Vanilla foundation build tool</param>
    public static void Update(BuildTool_Reform reformTool)
    {
        Vector3 cursorTarget = reformTool.cursorTarget;

        if(_start.HasValue)
        {
            if(VFInput.rtsCancel.onDown)
            {
                Reset(reformTool);
                VFInput.UseMouseRight();
            }
            else
            {
                SelectRange(reformTool, _start.Value, cursorTarget);
            }
        }
        else if(reformTool.cursorValid && VFInput._buildConfirm.onDown)
        {
            _start = cursorTarget;
        }

        // string info = $"C: {reformTool.brushColor}, T: {reformTool.brushType}, S: {reformTool.brushSize}, D: {reformTool.brushDecalType}";
        // string info = $"I: {cursorIndex}, S: {_start}, PC: {reformTool.cursorPointCount}";
        // reformTool.actionBuild.model.cursorText = point.ToString();
    }

    /// <summary>Reset to initial state</summary>
    public static void Reset(BuildTool_Reform reformTool)
    {
        _start = null;
        Array.Resize(ref reformTool.cursorPoints, 100);
        Array.Resize(ref reformTool.cursorIndices, 100);
    }

    private static void SelectRange(BuildTool_Reform reformTool, Vector3 start, Vector3 end)
        => reformTool.cursorPointCount = reformTool.planet.aux.ReformSnapRect(start, end, ref reformTool.cursorPoints, ref reformTool.cursorIndices, out reformTool.cursorTarget);
}
