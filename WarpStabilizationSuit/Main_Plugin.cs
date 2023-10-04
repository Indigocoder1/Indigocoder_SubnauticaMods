using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using System.IO;
using WarpStabilizationSuit.Items;
using BepInEx.Configuration;
using IndigocoderLib;
using BepInEx.Bootstrap;
using Nautilus.Handlers;

namespace WarpStabilizationSuit
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.WarpStabilizationSuit";
        private const string pluginName = "Warp Stabilization Suit";
        private const string versionString = "1.2.7";
        public static ManualLogSource logger;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ConfigEntry<bool> UseHardRecipe;
        public static bool TabsNeeded;

        private void Awake()
        {
            logger = Logger;

            if (PiracyDetector.TryFindPiracy())
            {
                return;
            }

            harmony.PatchAll();
            UseHardRecipe = Config.Bind("Warp Stabilization Suit", "Use harder recipe", false);

            new Suit_ModOptions();

            if(Chainloader.PluginInfos.ContainsKey("com.ramune.SeaglideUpgrades") || Chainloader.PluginInfos.ContainsKey("com.ramune.OrganizedWorkbench"))
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "Other", "Other", SpriteManager.Get(TechType.Titanium));
                TabsNeeded = true;
            }

            Gloves_Craftable.Patch();
            Suit_Craftable.Patch();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
