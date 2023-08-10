using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using IndigocoderLib;
using MirrorMod.Craftables;
using Nautilus.Utility;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MirrorMod
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.MirrorMod";
        private const string pluginName = "Mirror Mod";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public static ConfigEntry<int> MirrorResolution;

        public static AssetBundle assetBundle { get; private set; }

        private IEnumerator Start()
        {
            logger = Logger;

            PiracyDetector.TryFindPiracy();

            harmony.PatchAll();

            SetUpConfigs();

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "mirrorassetbundle"));

            new Mirror_ModOptions();

            yield return new WaitUntil(() => MaterialUtils.IsReady);

            Mirror_Variant1.Patch();
            Mirror_Variant2.Patch();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void SetUpConfigs()
        {
            MirrorResolution = Config.Bind("Mirror Mod", "Mirror resolution", 800,
                new ConfigDescription("How high quality the mirror resolution is (Requires restart | Higher = Worse performance)",
                acceptableValues: new AcceptableValueRange<int>(500, 1500)));
        }
    }
}
