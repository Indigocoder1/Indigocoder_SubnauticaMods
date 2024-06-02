using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TodoList.Patches;
using UnityEngine;

namespace TodoList
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("sn.subnauticamap.mod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TodoList";
        private const string pluginName = "Todo List";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle AssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "todolist"));
        public static TodoListSaveData SaveData { get; } = SaveDataHandler.RegisterSaveDataCache<TodoListSaveData>();

        public static Atlas.Sprite TodoListTabSprite;
        public static GameObject NewItemPrefab;

        public static ConfigEntry<bool> CreateHintTodoItems;

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static PDATab todoTab;
        internal static bool Initialized;

        private void Awake()
        {
            if (Initialized) return;

            logger = Logger;

            LanguageHandler.RegisterLocalizationFolder();
            todoTab = EnumHandler.AddEntry<PDATab>("TodoList");

            BindConfigs();
            CachePrefabs();

            new TodoOptions();

            if(BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("sn.subnauticamap.mod"))
            {
                RunCompatibilityPatches();
            }

            harmony.PatchAll();

            Initialized = true;
            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void BindConfigs()
        {
            CreateHintTodoItems = Config.Bind("Todo List", "Create guiding todo items", false,
                new ConfigDescription("Automatically create todo list items at key story points to give you hints on what to do"));
        }

        private void CachePrefabs()
        {
            TodoListTabSprite = new Atlas.Sprite(AssetBundle.LoadAsset<Texture2D>("todoListTabImage"));
            TodoListTabSprite.size = new Vector2(220, 220);
            NewItemPrefab = AssetBundle.LoadAsset<GameObject>("ChecklistItem");
        }

        private void RunCompatibilityPatches()
        {
            Type mapControllerType = Type.GetType("SubnauticaMap.Controller, SubnauticaMap");
            MethodBase methodBase = mapControllerType.GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo transpilerInfo = AccessTools.Method(typeof(MapModCompatibilityPatches), 
                nameof(MapModCompatibilityPatches.ControllerUpdate_Transpiler));
            
            harmony.Patch(methodBase, null, null, new HarmonyMethod(transpilerInfo));
        }

        public static List<StoryGoalTodoEntry> StoryGoalTodoEntries { get; internal set; } = new()
        {
            new("Trigger_PDAIntroEnd", new[] { "OnPDAIntroEnd1", "OnPDAIntroEnd2", "OnPDAIntroEnd3" }, true),
            new("Story_AuroraWarning4", new[] { "OnAuroraExplode1", "OnAuroraExplode2" }, true),
            new("OnPlayRadioBloodKelp29", new[] { "OnLifepod2RadioFinished" }, true),
            new("OnPlayRadioGrassy25", new[] { "OnLifepod3RadioFinished" }, true),
            new("OnPlayRadioRadiationSuit", new[] { "OnLifepod4RadioFinished" }, true),
            new("OnPlayRadioShallows22", new[] { "OnLifepod6RadioFinished" }, true),
            new("OnPlayRadioKelp28", new[] { "OnLifepod7RadioFinished" }, true),
            new("OnPlayRadioKoosh26", new[] { "OnLifepod12RadioFinished" }, true),
            new("OnPlayRadioMushroom24", new[] { "OnLifepod13RadioFinished" }, true),
            new("OnPlayRadioGrassy21", new[] { "OnLifepod17RadioFinished" }, true),
            new("OnPlayRadioSecondOfficer", new[] { "OnLifepod19RadioFinished" }, true),
            new("OnPlayRadioGrassy21", new[] { "OnLifepod17RadioFinished" }, true),
            new("OnPlayRadioSecondOfficer", new[] { "OnCaptainsCodeRadioFinished1", "OnCaptainsCodeRadioFinished2" }, true),
            new("OnPlayRadioSunbeam4", new[] { "OnSunbeamPreparingToLand" }, true),
            new("Precursor_Gun_DataDownload3", new[] { "OnLostRiverHintDownloaded1", "OnLostRiverHintDownloaded2" }, true),
            new("Emperor_Telepathic_Contact1", new[] { "OnEmperorFirstTelepathy" }, true),
            new("Goal_SecondarySystems", new[] { "OnLifepodRepaired1", "OnLifepodRepaired2" }, true),
            new("Goal_JellyCaveEntrance", new[] { "OnJellyHintGiven" }, true),
            new("AdviseSelfScan", new[] { "OnAdviseSelfScan" }, true)
        };

        public struct StoryGoalTodoEntry
        {
            public string key;
            public string[] entries;
            public bool localized;

            public StoryGoalTodoEntry(string key, string[] entries, bool localized)
            {
                this.key = key;
                this.entries = entries;
                this.localized = localized;
            }
        }
    }
}
