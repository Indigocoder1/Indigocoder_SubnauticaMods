using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using IndigocoderLib;

namespace GrappleItemPickup_BepInEx
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("Indigocoder.GrapplingArmUpgrade", BepInDependency.DependencyFlags.SoftDependency)]
    public class GrappleItemPickupPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> EnableMod;
        public static ConfigEntry<bool> WriteLogs;
        public static ConfigEntry<float> PickupDistance;

        private const string myGUID = "Indigocoder.GrappleItemPickup";
        private const string pluginName = "Grapple Item Pickup";
        private const string versionString = "2.2.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            harmony.Patch(AccessTools.Method(typeof(ExosuitGrapplingArm), "FixedUpdate"), 
               new HarmonyMethod(AccessTools.Method(typeof(GrappleItemPickupMod), nameof(GrappleItemPickupMod.GrapplingArm_Patch))));

            //Check for GrapplingArmUpgrade and patch that if it exists
            if (Chainloader.PluginInfos.ContainsKey("Indigocoder.GrapplingArmUpgrade"))
            {
                IndigocoderLib.Utilities.PatchIfExists(harmony, "GrapplingArmUpgrade_BepInEx", "GrapplingArmUpgrade_BepInEx.GrapplingArmUpgrade_Handler", "FixedUpdate",
                    null, new HarmonyMethod(AccessTools.Method(typeof(GrapplingArmUpgrade_FixedUpdate_Patch), nameof(GrapplingArmUpgrade_FixedUpdate_Patch.GrapplingArm_Patch))), null);
            }
            else
            {
                Logger.LogInfo($"Not able to patch GrapplingArmUpgrade because the mod was not found");
            }

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            EnableMod = Config.Bind("Grapple Item Pickup Options", "Enable", true);
            WriteLogs = Config.Bind("Grapple Item Pickup Options", "Write Logs", false);

            PickupDistance = Config.Bind("Grapple Item Pickup Options", "Pickup Distance", 1f,
                new ConfigDescription("How close to the grappling arm an item needs to be before it's picked up", acceptableValues: new AcceptableValueRange<float>(1f, 3f)));

            new GrappleItemPickupModOptions();
        }
    }
}
