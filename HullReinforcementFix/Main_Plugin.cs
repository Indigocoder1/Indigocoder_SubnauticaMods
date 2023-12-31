using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HullReinforcementFix.Craftables;

namespace HullReinforcementFix
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.HullReinforcementFix";
        private const string pluginName = "Hull Reinforcement Fix";
        private const string versionString = "1.2.4";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ConfigEntry<bool> WriteLogs;
        public static ConfigEntry<float> MK1DamageReductionMultiplier;
        public static ConfigEntry<float> MK2DamageReductionMultiplier;
        public static ConfigEntry<float> MK3DamageReductionMultiplier;

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            UpgradedHullReinfocement.Patch(2);
            UpgradedHullReinfocement.Patch(3);

            harmony.PatchAll();

            SetupConfigs();
            new Hull_ModOptions();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
        private void SetupConfigs()
        {
            WriteLogs = Config.Bind("Hull Reinforcement Fix", "Write Logs", false);

            MK1DamageReductionMultiplier = Config.Bind("Hull Reinforcement Fix", "MK1 Damage Reduction Multiplier", 1f,
                new ConfigDescription("The damage reduction multiplier for the mk1 module", acceptableValues: new AcceptableValueRange<float>(1f, 2.5f)));

            MK2DamageReductionMultiplier = Config.Bind("Hull Reinforcement Fix", "MK2 Damage Reduction Multiplier", 1f,
                new ConfigDescription("The damage reduction multiplier for the mk2 module", acceptableValues: new AcceptableValueRange<float>(1f, 2.5f)));

            MK3DamageReductionMultiplier = Config.Bind("Hull Reinforcement Fix", "MK3 Damage Reduction Multiplier", 1f,
                new ConfigDescription("The damage reduction multiplier for the mk3 module", acceptableValues: new AcceptableValueRange<float>(1f, 2.5f)));
        }
    }
}
