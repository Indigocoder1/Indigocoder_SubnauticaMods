using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
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

        public static ConfigEntry<int> MirrorTextureSize;

        private IEnumerator Start()
        {
            logger = Logger;

            harmony.PatchAll();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");

            MirrorTextureSize = Config.Bind("Mirror Mod", "Mirror texture size", 800,
                new ConfigDescription("How high quality the texture is (Requires restart | Higher = Worse performance)",
                acceptableValues: new AcceptableValueRange<int>(500, 1500)));

            yield return new WaitUntil(() => MaterialUtils.IsReady);

            Mirror_Craftable.Patch();
        }
    }
}
