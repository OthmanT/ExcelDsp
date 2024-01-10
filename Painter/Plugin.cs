using BepInEx;

namespace ExcellentDsp.Painter;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
        => Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} loaded");

    public void OnDestroy()
        => Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} unloaded");
}
