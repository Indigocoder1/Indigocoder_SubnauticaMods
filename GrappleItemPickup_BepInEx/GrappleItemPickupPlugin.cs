using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace GrappleItemPickup_BepInEx
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class GrappleItemPickupPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> EnableMod;
        public static ConfigEntry<bool> WriteLogs;
        public static ConfigEntry<float> PickupDistance;

        private const string myGUID = "Indigocoder.GrappleItemPickup";
        private const string pluginName = "Grapple Item Pickup";
        private const string versionString = "1.1.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            harmony.PatchAll();
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            EnableMod = Config.Bind("Grapple Item Pickup Options", "Enable", true);
            WriteLogs = Config.Bind("Grapple Item Pickup Options", "Write Logs", false);

            PickupDistance = Config.Bind("Grapple Item Pickup Options", "Pickup Distance", 1f,
                new ConfigDescription("How close to the grappling arm an item needs to be before it's picked up", acceptableValues: new AcceptableValueRange<float>(1f, 3f)));

            new GrappleItemPickupModOptions();
        }
    }
}
