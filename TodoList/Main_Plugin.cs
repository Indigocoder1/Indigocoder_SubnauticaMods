using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System;
using System.Collections;
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

        private void Awake()
        {
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
            logger.LogInfo($"Method base = {methodBase}");
            MethodInfo transpilerInfo = AccessTools.Method(typeof(MapModCompatibilityPatches), 
                nameof(MapModCompatibilityPatches.ControllerUpdate_Transpiler));
            
            harmony.Patch(methodBase, null, null, new HarmonyMethod(transpilerInfo));
        }

        public static List<StoryGoalTodoEntry> StoryGoalTodoEntries { get; internal set; } = new()
        {
            new("Trigger_PDAIntroEnd", new[] { "OnPDAIntroEnd1", "OnPDAIntroEnd2", "OnPDAIntroEnd3" }, true),
            new("Trigger_PDAIntroEnd", new[] { "OnAuroraExplode" }, true),
            new("OnPlayRadioBloodKelp29", new[] { "OnLifepod2RadioFinished" }, true),
            new("OnPlayRadioGrassy25", new[] { "OnLifepod3RadioFinished" }, true),
            new("OnPlayRadioRadiationSuit", new[] { "OnLifepod4RadioFinished" }, true),
            new("OnPlayRadioShallows22", new[] { "OnLifepod6RadioFinished" }, true),
            new("OnPlayRadioKelp28", new[] { "OnLifepod7RadioFinished" }, true),
            new("OnPlayRadioKoosh26", new[] { "OnLifepod12RadioFinished" }, true),
            new("OnPlayRadioGrassy21", new[] { "OnLifepod17RadioFinished" }, true),
            new("OnPlayRadioSecondOfficer", new[] { "OnLifepod19RadioFinished" }, true),
            new("OnPlayRadioGrassy21", new[] { "OnLifepod17RadioFinished" }, true),
            new("OnPlayRadioSecondOfficer", new[] { "OnCaptainsCodeRadioFinished1", "OnCaptainsCodeRadioFinished2" }, true),
            new("OnPlayRadioSunbeam4", new[] { "OnSunbeamPreparingToLand" }, true),
            new("Precursor_Gun_DataDownload3", new[] { "OnLostRiverHintDownloaded1", "OnLostRiverHintDownloaded2" }, true),
            new("Emperor_Telepathic_Contact1", new[] { "OnEmperorFirstTelepathy" }, true),
        };

        /*
        public static Dictionary<string, string[]> StoryGoalTodoEntries { get; internal set; } = new()
        {
            { "Trigger_PDAIntroEnd", new[] { "Craft a Scanner Tool", "Repair the Lifepod's secondary systems", "Repair the Lifepod radio" } },
            { "Story_AuroraWarning4", new[] { "Repair the Aurora" } },
            { "OnPlayRadioBloodKelp29", new[] { "Investigate Lifepod 2" } },
            { "OnPlayRadioGrassy25", new[] { "Investigate Lifepod 3" } },
            { "OnPlayRadioRadiationSuit", new[] { "Investigate Lifepod 4" } },
            { "OnPlayRadioShallows22", new[] { "Investigate Lifepod 6" } },
            { "OnPlayRadioKelp28", new[] { "Investigate Lifepod 7" } },
            { "OnPlayRadioKoosh26", new[] { "Investigate Lifepod 12" } },
            { "OnPlayRadioGrassy21", new[] { "Investigate Lifepod 17" } },
            { "OnPlayRadioSecondOfficer", new[] { "Investigate Officer Keen's Lifepod (Lifepod 19)" } },
            { "OnPlayRadioCaptainsQuartersCode", new[] { "Investigate the Aurora's captain's quarters", "Get a sandwich (The regular)" } },
            { "OnPlayRadioSunbeam4", new[] { "Visit the Sunbeam landing site" } },
            { "Precursor_Gun_DataDownload3", new[] { "Search for the Disease Research Facility", "Search for the Thermal Power Facility" } },
            { "Emperor_Telepathic_Contact1", new[] { "Find the source of the telepathic vision" } },
        };
        */

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
