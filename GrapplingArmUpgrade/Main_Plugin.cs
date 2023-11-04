using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using IndigocoderLib;
using BepInEx.Bootstrap;
using Nautilus.Handlers;

namespace GrapplingArmUpgrade_BepInEx
{
    //Please note that this mod is heavily inspired by a mod made by zorgesho#9929
    //You can find his mod on NexusMods here: https://www.nexusmods.com/subnautica/mods/368
    //At time of writing the mod only works for Legacy which is the reason for this port

    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)] //Thanks Ramune!
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInProcess("Subnautica.exe")]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.GrapplingArmUpgrade";
        private const string pluginName = "Grappling Arm Upgrade";
        private const string versionString = "1.1.2";

        public static ConfigEntry<bool> EnableMod;
        public static ConfigEntry<float> ArmCooldown;
        public static ConfigEntry<float> HookMaxDistance;
        public static ConfigEntry<float> InitialHookSpeed;
        public static ConfigEntry<float> ExosuitAcceleration;
        public static ConfigEntry<float> AttachedObjectAcceleration;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;
        public static bool TabsNeeded;

        private void Awake()
        {
            PiracyDetector.TryFindPiracy();

            logger = Logger;

            /*
            if (Chainloader.PluginInfos.ContainsKey("com.ramune.SeaglideUpgrades") || Chainloader.PluginInfos.ContainsKey("com.ramune.OrganizedWorkbench"))
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "Other", "Other", SpriteManager.Get(TechType.Titanium));
                TabsNeeded = true;
            }
            */

            UpgradedArm_Craftable.RegisterModule();
            harmony.PatchAll();

            SetUpConfigs();
            new GrapplingArmUpgrade_Options();

            logger.LogInfo("Patching module class");
        }

        private void SetUpConfigs()
        {
            EnableMod = Config.Bind("Grappling Arm Upgrade Options", "Enable", true);

            ArmCooldown = Config.Bind("Grappling Arm Upgrade Options", "Grappling Use Cooldown", 0.5f,
                new ConfigDescription("Time in seconds before you can grapple again after grappling",
                acceptableValues: new AcceptableValueRange<float>(0f, 5f)));
            HookMaxDistance = Config.Bind("Grappling Arm Upgrade Options", "Grappling Hook Max Distance", 50f,
                new ConfigDescription("Max distance the grappling hook can travel before resetting",
                acceptableValues: new AcceptableValueRange<float>(35f, 100f)));
            InitialHookSpeed = Config.Bind("Grappling Arm Upgrade Options", "Initial Grappling Hook Speed", 50f,
                new ConfigDescription("The amount of force with which the grappling travels is shot through the air",
                acceptableValues: new AcceptableValueRange<float>(25f, 100f)));
            ExosuitAcceleration = Config.Bind("Grappling Arm Upgrade Options", "Prawn Grappling Acceleration", 20f,
                new ConfigDescription("How much acceleration is given to the Prawn while grappling",
                acceptableValues: new AcceptableValueRange<float>(15f, 30f)));
            AttachedObjectAcceleration = Config.Bind("Grappling Arm Upgrade Options", "Attached Object Acceleration", 600f,
                new ConfigDescription("How much accleration is given to the object your hook is attached to",
                acceptableValues: new AcceptableValueRange<float>(400f, 1000f)));
        }
    }
}
