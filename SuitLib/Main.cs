using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SuitLib
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    internal class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.SuitLib";
        private const string pluginName = "SuitLib";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
        {
            logger = Logger;

            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
