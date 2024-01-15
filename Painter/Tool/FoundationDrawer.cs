using ExcelDsp.Painter.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace ExcelDsp.Painter.Tool;

/// <summary>Draws foundation tiles</summary>
internal static class FoundationDrawer
{
    private static Vector3? _start;

    /// <summary>Whether this is currently enabled, overriding the vanilla foundation logic</summary>
    public static bool IsEnabled { get; set; }

    public static void UpdateRaycast(BuildTool_Reform reformTool)
    {
        reformTool.brushSize = 1;
        reformTool.cursorValid = Raycast(reformTool.mouseRay, out Vector3 target);
        SelectRange(reformTool, target, target);

        reformTool.controller.cmd = reformTool.controller.cmd with
        {
            test = reformTool.cursorTarget,
            target = reformTool.cursorTarget,
            state = reformTool.cursorValid ? 1 : 0,
        };
    }

    /// <summary>Update tool logic each frame (when enabled and active)</summary>
    /// <param name="reformTool">Vanilla foundation build tool</param>
    public static void UpdateAction(BuildTool_Reform reformTool)
    {
        if(_start.HasValue)
        {
            if(VFInput.rtsCancel.onDown)
            {
                Reset(reformTool);
                VFInput.UseMouseRight();
            }
            else
            {
                SelectRange(reformTool, _start.Value, reformTool.cursorTarget);
            }
        }
        else if(reformTool.cursorValid && VFInput._buildConfirm.onDown)
        {
            _start = reformTool.cursorTarget;
        }

        if(!VFInput.onGUIOperate)
        {
            UICursor.SetCursor(ECursor.Reform);

            if(reformTool.cursorValid)
            {
                // string info = $"C: {reformTool.brushColor}, T: {reformTool.brushType}, S: {reformTool.brushSize}, D: {reformTool.brushDecalType}";
                int zero = reformTool.cursorIndices.Count(i => i == 0);
                int neg = reformTool.cursorIndices.Count(i => i < 0);
                int pos = reformTool.cursorIndices.Count(i => i > 0);
                string info = $"Point: {reformTool.cursorPointCount}, Pos: {pos}, Neg: {neg}, Zero: {zero}";
                reformTool.actionBuild.model.cursorText = info;
            }
        }
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

    private static bool Raycast(Ray ray, out Vector3 target)
    {
        target = Vector3.zero;
        if(VFInput.onGUIOperate || !VFInput.inScreen)
            return false;
        ;

        int layerMask = 528;
        bool hitGround = Physics.Raycast(ray, out RaycastHit hitInfo, 400f, layerMask, QueryTriggerInteraction.Collide);

        if(!hitGround)
        {
            Ray inverseRay = new(ray.GetPoint(200f), -ray.direction);
            hitGround = Physics.Raycast(inverseRay, out hitInfo, 200f, layerMask, QueryTriggerInteraction.Collide);
        }

        if(!hitGround)
            return true;

        target = hitInfo.point;
        return true;
    }
}
