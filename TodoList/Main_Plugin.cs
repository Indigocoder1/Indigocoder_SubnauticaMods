using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Json.Converters;
using Nautilus.Utility;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TodoList
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TodoList";
        private const string pluginName = "Todo List";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle AssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "todolist"));

        public static Atlas.Sprite TodoListTabSprite;
        public static GameObject NewItemPrefab;

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static PDATab todoTab;

        private void Awake()
        {
            logger = Logger;
            //new Options();

            LanguageHandler.RegisterLocalizationFolder();
            todoTab = EnumHandler.AddEntry<PDATab>("TodoList");

            CachePrefabs(); 

            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void CachePrefabs()
        {
            TodoListTabSprite = new Atlas.Sprite(AssetBundle.LoadAsset<Texture2D>("todoListTabImage"));
            NewItemPrefab = AssetBundle.LoadAsset<GameObject>("ChecklistItem");
        }
    }
}
