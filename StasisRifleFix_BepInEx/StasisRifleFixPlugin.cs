using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace StasisRifleFixMod_BepInEx
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class StasisFreezeFixPlugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.StasisRifleFix";
        private const string pluginName = "Stasis Rifle Fix";
        private const string versionString = "1.2.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        private void Awake()
        {
            new StasisRifleFixModOptions();

            harmony.PatchAll();
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
            logger = Logger;
        }
    }
}
