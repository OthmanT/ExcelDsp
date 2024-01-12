namespace ExcelDsp.Painter.Tool;

/// <summary>Draws foundation tiles</summary>
internal static class FoundationDrawer
{
    /// <summary>Whether this is currently enabled, overriding the vanilla foundation logic</summary>
    public static bool IsEnabled { get; set; }

    /// <summary>Update each frame (when enabled and active)</summary>
    /// <param name="reformTool">Vanilla foundation build tool</param>
    public static void Update(BuildTool_Reform reformTool)
    {
        string info = $"C: {reformTool.brushColor}, T: {reformTool.brushType}, S: {reformTool.brushSize}, D: {reformTool.brushDecalType}";
        UIRealtimeTip.Popup(info);
    }
}
