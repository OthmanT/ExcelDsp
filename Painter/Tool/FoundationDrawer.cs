namespace ExcelDsp.Painter.Tool;

/// <summary>Draws foundation tiles</summary>
internal static class FoundationDrawer
{
    private static int? _start;

    /// <summary>Whether this is currently enabled, overriding the vanilla foundation logic</summary>
    public static bool IsEnabled { get; set; }

    /// <summary>Update tool logic each frame (when enabled and active)</summary>
    /// <param name="reformTool">Vanilla foundation build tool</param>
    public static void Update(BuildTool_Reform reformTool)
    {
        // Array.Clear(reformTool.cursorPoints, 0, reformTool.cursorPoints.Length);
        // Array.Clear(reformTool.cursorIndices, 0, reformTool.cursorIndices.Length);

        int cursorIndex = reformTool.cursorIndices[0];

        if(_start.HasValue)
            reformTool.cursorIndices[1] = _start.Value;
        else if(reformTool.cursorValid && !VFInput.onGUIOperate && VFInput._buildConfirm.onDown)
            _start = cursorIndex;

        // string info = $"C: {reformTool.brushColor}, T: {reformTool.brushType}, S: {reformTool.brushSize}, D: {reformTool.brushDecalType}";
        string info = $"I: {cursorIndex}, S: {_start}, PC: {reformTool.cursorPointCount}";
        reformTool.actionBuild.model.cursorText = info;
    }

    /// <summary>Reset to initial state</summary>
    public static void Reset()
        => _start = null;
}
