using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using System.IO;
using WarpStabilizationSuit.Items;
using BepInEx.Configuration;
using IndigocoderLib;

namespace WarpStabilizationSuit
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.WarpStabilizationSuit";
        private const string pluginName = "Warp Stabilization Suit";
        private const string versionString = "1.2.6";

        public static ManualLogSource logger;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ConfigEntry<bool> UseHardRecipe;

        private void Awake()
        {
            logger = Logger;

            if (PiracyDetector.TryFindPiracy())
            {
                return;
            }

            harmony.PatchAll();
            UseHardRecipe = Config.Bind("Warp Stabilization Suit", "Use harder recipe", false, new ConfigDescription("Requires restart"));

            new Suit_ModOptions();

            Gloves_Craftable.Patch();
            Suit_Craftable.Patch();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
