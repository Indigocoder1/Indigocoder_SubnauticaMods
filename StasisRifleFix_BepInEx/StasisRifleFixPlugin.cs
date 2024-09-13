using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace StasisRifleFixMod_BepInEx
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> WriteLogs;

        private const string myGUID = "Indigocoder.StasisRifleFix";
        private const string pluginName = "Stasis Rifle Fix";
        private const string versionString = "2.5.3";
        
        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            harmony.PatchAll();
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            WriteLogs = Config.Bind("Stasis Rifle Fix Options", "Write Logs", false);

            new StasisRifleFixModOptions();
        }
    }
}
