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
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.MirrorMod";
        private const string pluginName = "Mirror Mod";
        private const string versionString = "1.1.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public static ConfigEntry<int> MirrorResolution;
        public static ConfigEntry<KeyCode> ResizeHandleToggleButton;

        public static AssetBundle assetBundle { get; private set; }

        private IEnumerator Start()
        {
            logger = Logger;

            PiracyDetector.TryFindPiracy();

            harmony.PatchAll();
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "mirrorassetbundle"));

            SetUpConfigs();
            new Mirror_ModOptions();

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

            //No idea why but I can't get the resize handles working
            /*
            Atlas.Sprite variant3Sprite = ImageHelper.GetSpriteFromAssetsFolder("MirrorVariant2.png");
            GameObject variant3Prefab = assetBundle.LoadAsset<GameObject>("ResizableMirror");
            Basic_Mirror.Patch("MirrorVariant3", "Resizable Mirror", variant3Sprite, variant3Prefab);
            */
        }

        private void SetUpConfigs()
        {
            MirrorResolution = Config.Bind("Mirror Mod", "Mirror resolution", 800,
                new ConfigDescription("How high quality the mirror resolution is (Requires restart | Higher = Worse performance)",
                acceptableValues: new AcceptableValueRange<int>(500, 2000)));

            ResizeHandleToggleButton = Config.Bind("Mirror Mod", "Resize handle toggle button", KeyCode.I, 
                new ConfigDescription("The button that disables the resize handles on the reizable mirror"));
        }
    }
}
