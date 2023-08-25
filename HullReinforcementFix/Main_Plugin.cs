using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace HullReinforcementFix
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.HullReinforcementFix";
        private const string pluginName = "Hull Reinforcement Fix";
        private const string versionString = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ConfigEntry<bool> WriteLogs;
        public static ConfigEntry<float> DamageReductionMultiplier;

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            harmony.PatchAll();

            SetupConfigs();
            new Hull_ModOptions();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void SetupConfigs()
        {
            WriteLogs = Config.Bind("Hull Reinforcement Fix", "Write Logs", false);

            DamageReductionMultiplier = Config.Bind("Hull Reinforcement Fix", "Damage Reduction Multiplier", 1f,
                new ConfigDescription("How much damage is reduced per module", acceptableValues: new AcceptableValueRange<float>(0.1f, 10)));
        }
    }
}
