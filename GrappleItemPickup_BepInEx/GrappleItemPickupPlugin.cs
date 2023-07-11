using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace GrappleItemPickup_BepInEx
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class GrappleItemPickupPlugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.GrappleItemPickup";
        private const string pluginName = "Grapple Item Pickup";
        private const string versionString = "1.1.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        private void Awake()
        {
            new GrappleItemPickupModOptions();

            harmony.PatchAll();
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
            logger = Logger;
        }
    }
}
