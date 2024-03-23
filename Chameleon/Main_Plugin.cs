using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IndigocoderLib;
using System.IO;
using System.Reflection;

namespace Chameleon
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.Chameleon";
        private const string pluginName = "Chameleon Sub";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        //public static AssetBundle assetBundle { get; private set; }

        private void Awake()
        {
            logger = Logger;

            PiracyDetector.TryFindPiracy();
            harmony.PatchAll();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
