using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Reflection;
using BepInEx.Configuration;

namespace GrapplingArmUpgrade_BepInEx
{
    //Please note that this mod is heavily inspired by a mod made by zorgesho#9929
    //You can find his mod on NexusMods here: https://www.nexusmods.com/subnautica/mods/368
    //At time of writing the mod only works for Legacy which is the reason for this port

    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> EnableMod;
        public static ConfigEntry<float> ArmCooldown;
        public static ConfigEntry<float> HookMaxDistance;
        public static ConfigEntry<float> HookSpeed;
        public static ConfigEntry<float> ExosuitAcceleration;
        public static ConfigEntry<float> HookShootForce;

        private const string myGUID = "Indigocoder.GrapplingArmUpgrade";
        private const string pluginName = "Grappling Arm Upgrade";
        private const string versionString = "1.0.0";

        private static Assembly assembly { get; } = Assembly.GetExecutingAssembly();

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            Harmony.CreateAndPatchAll(assembly, $"{myGUID}");
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            EnableMod = Config.Bind("Grappling Arm Upgrade Options", "Enable", true);

            ArmCooldown = Config.Bind("Grappling Arm Upgrade Options", "Grappling Use Cooldown", 0.5f,
                new ConfigDescription("Time in seconds before you can grapple again after grappling", acceptableValues: new AcceptableValueRange<float>(0f, 5f)));
            HookMaxDistance = Config.Bind("Grappling Arm Upgrade Options", "Grappling Hook Max Distance", 50f,
                new ConfigDescription("Max distance the grappling hook can travel before resetting", acceptableValues: new AcceptableValueRange<float>(35f, 100f)));
            HookSpeed = Config.Bind("Grappling Arm Upgrade Options", "Grappling Hook Speed", 50f,
                new ConfigDescription("The amount of force with which the grappling travels through the air", acceptableValues: new AcceptableValueRange<float>(25f, 70f)));
            ExosuitAcceleration = Config.Bind("Grappling Arm Upgrade Options", "Prawn Grappling Acceleration", 20f,
                new ConfigDescription("How much acceleration is given to the Prawn while grappling", acceptableValues: new AcceptableValueRange<float>(15f, 50f)));
            HookShootForce = Config.Bind("Grappling Arm Upgrade Options", "Grappling Hook Shoot Force", 600f,
                new ConfigDescription("The amount of force with which the grappling hook is shot", acceptableValues: new AcceptableValueRange<float>(400f, 1000f)));

            new GrapplingArmUpgrade_Options();
        }
    }
}
