using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace UpgradedJumpJetModule
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.UpgradedJumpJetModule";
        private const string pluginName = "Upgraded Jump Jet Module";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ConfigEntry<float> UpgradedJetForce;
        public static ConfigEntry<float> UpgradedJetAcceleration;
        public static ConfigEntry<float> UpgradedJumpForce;

        public static bool TabsNeeded;

        private void Awake()
        {
            logger = Logger;

            if (Chainloader.PluginInfos.ContainsKey("com.ramune.SeaglideUpgrades") || Chainloader.PluginInfos.ContainsKey("com.ramune.OrganizedWorkbench"))
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "Other", "Other", SpriteManager.Get(TechType.Titanium));
                TabsNeeded = true;
            }

            UpgradedJetsModule.RegisterModule();
            harmony.PatchAll();

            UpgradedJetForce = Config.Bind("Upgraded Jump Jet Module", "Jet force", 10f,
                new ConfigDescription("How much force you have while holding the jump button", acceptableValues: new AcceptableValueRange<float>(8f, 20f)));
            UpgradedJetAcceleration = Config.Bind("Upgraded Jump Jet Module", "Prawn acceleration", 0.5f,
                new ConfigDescription("How much you accelerate while in the air", acceptableValues: new AcceptableValueRange<float>(0.3f, 2f)));
            UpgradedJumpForce = Config.Bind("Upgraded Jump Jet Module", "Jump force", 10f,
                new ConfigDescription("How much force you push off from the ground with", acceptableValues: new AcceptableValueRange<float>(5f, 20f)));

            new UpgradedJumpJet_ModOptions();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
