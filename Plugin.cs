using BepInEx;
using EnhancedIcons;
using HarmonyLib;

namespace EnhancedIcons
{
    [BepInPlugin(PluginInfo.PluginGuid, PluginInfo.PluginName, PluginInfo.PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;

        private void Awake()
        {
            _harmony = new Harmony(PluginInfo.PluginGuid);
            _harmony.PatchAll(typeof(InventoryIconPatch));
            Logger.LogInfo("Plugin EnhancedIcons is loaded!");
        }
    }
}