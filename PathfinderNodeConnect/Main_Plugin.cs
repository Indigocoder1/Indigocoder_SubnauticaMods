using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PathfinderNodeConnect
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.PathfinderNodeConnect";
        private const string pluginName = "Pathfinder Node Connect";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle assetBundle { get; private set; }

        private void Awake()
        {
            logger = Logger;

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "lrAssetBundle"));
            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
