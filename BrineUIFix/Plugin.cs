using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace BrineUIFix
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.BrineUIFix";
        private const string pluginName = "Brine UI Fix";
        private const string versionString = "1.0.0";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}