using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TextureReplacerEditor
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("Indigocoder.TextureReplacer")]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TextureReplacerEditor";
        private const string pluginName = "Texture Replacer Editor";
        private const string versionString = "1.0.1";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle AssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "texturereplacereditor"));
        public static string TextureReplacerAssetsFolderPath;

        internal static GameObject CurrentEditorWindowInstance;

        private void Awake()
        {
            logger = Logger;

            string parentFolderName = Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName).FullName;
            TextureReplacerAssetsFolderPath = Path.Combine(new string[] { parentFolderName, "TextureReplacer", "Assets" });

            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
