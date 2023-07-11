using BepInEx;
using HarmonyLib;
using BepInEx.Logging;

namespace GrapplingArmUpgrade_BepInEx
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.GrapplingArmUpgrade";
        private const string pluginName = "Grappling Arm Upgrade";
        private const string versionString = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;
            harmony.PatchAll();
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
