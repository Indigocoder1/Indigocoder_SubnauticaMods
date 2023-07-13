using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Reflection;

namespace GrapplingArmUpgrade_BepInEx
{
    //Please note that this mod is heavily inspired by a mod made by zorgesho#9929
    //You can find his mod on NexusMods here: https://www.nexusmods.com/subnautica/mods/368
    //At time of writing the mod only works for Legacy which is the reason for this port

    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
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
        }
    }
}
