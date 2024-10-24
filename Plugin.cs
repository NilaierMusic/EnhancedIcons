using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedIcons
{
    [BepInPlugin(PluginInfo.PluginGuid, PluginInfo.PluginName, PluginInfo.PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;
        private bool _isBagConfigPresent;

        private void Awake()
        {
            _harmony = new Harmony(PluginInfo.PluginGuid);
            _harmony.PatchAll(typeof(InventoryIconPatch));
            Logger.LogInfo("Plugin EnhancedIcons is loaded!");

            DetectBagConfig();
        }

        private void DetectBagConfig()
        {
            try
            {
                var bagConfigAssembly = Assembly.Load("LTC_BagConfig");
                if (bagConfigAssembly != null)
                {
                    _isBagConfigPresent = true;
                    Logger.LogInfo("BagConfig mod detected.");
                }
            }
            catch (Exception)
            {
                _isBagConfigPresent = false;
                Logger.LogInfo("BagConfig mod not detected.");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class BagConfigDependentAttribute : Attribute
    {
    }
}
