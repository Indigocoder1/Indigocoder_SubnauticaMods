using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomCraftGUI
{
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.CustomCraftGUI";
        private const string pluginName = "Custom Craft GUI";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle assetBundle { get; private set; }

        private void Awake()
        {
            logger = Logger;

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "customcraftgui"));
            harmony.PatchAll();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
