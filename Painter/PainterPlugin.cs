using BepInEx;
using HarmonyLib;

namespace ExcellentDsp.Painter;

/// <summary>Plugin for DSP for painting foundations</summary>
[BepInPlugin(Id, Name, Shared.Version)]
public class PainterPlugin : BaseUnityPlugin
{
    private Harmony? _harmony;

    /// <summary>Unique id</summary>
    public const string Id = "055eeac2-b236-46ae-9ced-26f582094ead";

    /// <summary>Display name</summary>
    public const string Name = "Painter";

    /// <summary>First update after loading</summary>
    public void Awake()
    {
        Shared.Logger = Logger;
        Logger.LogInfo($"Plugin {Name} v{Shared.Version} loaded");

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
        Logger.LogInfo($"Plugin {Name} v{Shared.Version} unloaded");

        _harmony?.UnpatchSelf();
        _harmony = null;
    }
#endif
}
