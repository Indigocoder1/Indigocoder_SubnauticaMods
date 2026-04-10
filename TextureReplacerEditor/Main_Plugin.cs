using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using Nautilus.Handlers;
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
        private const string versionString = "1.0.4";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle AssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "texturereplacereditor"));
        public static string TextureReplacerAssetsFolderPath;

        internal static GameObject CurrentEditorWindowInstance;
        internal static GameInput.Button EditButton;

        private void Awake()
        {
            logger = Logger;

            string parentFolderName = Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName).FullName;
            TextureReplacerAssetsFolderPath = Path.Combine(new string[] { parentFolderName, "TextureReplacer", "Assets" });

            harmony.PatchAll();
            
            EditButton = EnumHandler.AddEntry<GameInput.Button>("Activate edit mode")
                .CreateInput()
                .WithBinding(GameInput.Device.Keyboard, GameInputHandler.Paths.Mouse.MiddleButton)
                .WithCategory("PrototypeInputCategory");

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
