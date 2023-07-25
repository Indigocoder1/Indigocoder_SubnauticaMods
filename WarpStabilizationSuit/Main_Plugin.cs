using HarmonyLib;
using BepInEx;
using BepInEx.Logging;

namespace WarpStabilizationSuit
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.WarpStabilizationSuit";
        private const string pluginName = "Warp Stabilization Suit";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
        {
            logger = Logger;

            harmony.PatchAll();
            Suit_Craftable.Patch();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
