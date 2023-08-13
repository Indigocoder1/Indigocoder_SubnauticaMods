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
            new Mirror_ModOptions();

            PiracyDetector.TryFindPiracy();

            harmony.PatchAll();
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "mirrorassetbundle"));

            SetUpConfigs();

            yield return new WaitUntil(() => MaterialUtils.IsReady);

            SetUpMirrors();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void SetUpMirrors()
        {
            Atlas.Sprite variant1Sprite = ImageHelper.GetSpriteFromAssetsFolder("MirrorVariant1.png");
            GameObject variant1Prefab = assetBundle.LoadAsset<GameObject>("Mirror_Variant1");
            Basic_Mirror.Patch("MirrorVariant1", "Mirror (Variant 1)", variant1Sprite, variant1Prefab);

            Atlas.Sprite variant2Sprite = ImageHelper.GetSpriteFromAssetsFolder("MirrorVariant2.png");
            GameObject variant2Prefab = assetBundle.LoadAsset<GameObject>("Mirror_Variant2");
            Basic_Mirror.Patch("MirrorVariant2", "Mirror (Variant 2)", variant2Sprite, variant2Prefab);
        }

        private void SetUpConfigs()
        {
            MirrorResolution = Config.Bind("Mirror Mod", "Mirror resolution", 800,
                new ConfigDescription("How high quality the mirror resolution is (Requires restart | Higher = Worse performance)",
                acceptableValues: new AcceptableValueRange<int>(500, 1500)));
        }
    }
}
