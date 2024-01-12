using BepInEx;
using BepInEx.Configuration;
using ExcelDsp.Painter.Tool;
using HarmonyLib;

namespace ExcelDsp.Painter;

/// <summary>Plugin for DSP for painting foundations</summary>
[BepInPlugin(Id, Name, Shared.Version)]
public class PainterPlugin : BaseUnityPlugin
{
    private Harmony? _harmony;
    private ConfigEntry<KeyboardShortcut>? _enableKey;

    /// <summary>Display name</summary>
    public const string Name = "Foundation Painter";

    /// <summary>Unique id</summary>
    public const string Id = "ExcelDsp.Painter";

    /// <summary>First update after loading</summary>
    public void Awake()
    {
        Shared.Logger = Logger;
        Logger.LogInfo($"{Name} v{Shared.Version} loaded");

        _enableKey = Config.Bind("Keyboard Shortcuts", "EnableKey", KeyboardShortcut.Deserialize("D + LeftControl"), "Keyboard shortcut to enable/disable foundation painting");

        _harmony = new Harmony(Id);
        _harmony.PatchAll(typeof(PainterPlugin).Assembly);

#if DEBUG
        Logger.LogDebug($"Debug build");
#else
        _harmony = null;
#endif
    }

#if DEBUG
    /// <summary>Unloading (for ScriptEngine)</summary>
    public void OnDestroy()
    {
        Logger.LogInfo($"{Name} v{Shared.Version} unloaded");

        _harmony?.UnpatchSelf();
        _harmony = null;
    }
#endif

    /// <summary>Update game logic each frame</summary>
    public void Update()
    {
        if(_enableKey is not null && _enableKey.Value.IsDown())
        {
            FoundationDrawer.IsEnabled = !FoundationDrawer.IsEnabled;
            Logger.LogDebug($"Enabled: {FoundationDrawer.IsEnabled}");
        }
    }
}
