using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using TodoList.Patches;
using UnityEngine;
using static TodoList.Main_Plugin;

namespace TodoList
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("sn.subnauticamap.mod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TodoList";
        private const string pluginName = "Todo List";
        private const string versionString = "1.0.1";

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
            new("Trigger_PDAIntroEnd", new EntryInfo[] { 
                new("OnPDAIntroEnd1", "Goal_Scanner", true), 
                new("OnPDAIntroEnd2", "RepairLifepod", true), 
                new("OnPDAIntroEnd3", null, true)}),
            new("Story_AuroraWarning4", new EntryInfo[] { new("OnAuroraExplode1", null, true), new("OnAuroraExplode2", null, true)}),
            new("OnPlayRadioBloodKelp29", new EntryInfo[] { new("OnLifepod2RadioFinished", null, true)}),
            new("OnPlayRadioGrassy25", new EntryInfo[] { new("OnLifepod3RadioFinished", null, true)}),
            new("OnPlayRadioRadiationSuit", new EntryInfo[] { new("OnLifepod4RadioFinished", null, true)}),
            new("OnPlayRadioShallows22", new EntryInfo[] { new("OnLifepod6RadioFinished", null, true)}),
            new("OnPlayRadioKelp28", new EntryInfo[] { new("OnLifepod7RadioFinished", null, true)}),
            new("OnPlayRadioKoosh26", new EntryInfo[] { new("OnLifepod12RadioFinished", null, true)}),
            new("OnPlayRadioMushroom24", new EntryInfo[] { new("OnLifepod13RadioFinished", null, true)}),
            new("OnPlayRadioGrassy21", new EntryInfo[] { new("OnLifepod17RadioFinished", null, true)}),
            new("OnPlayRadioSecondOfficer", new EntryInfo[] { new("OnLifepod19RadioFinished", null, true)}),
            new("OnPlayRadioSecondOfficer", new EntryInfo[] { new("OnCaptainsCodeRadioFinished1", null, true), new("OnCaptainsCodeRadioFinished2", null, true)}),
            new("OnPlayRadioSunbeam4", new EntryInfo[] { new("OnSunbeamPreparingToLand", null, true)}),
            new("Precursor_Gun_DataDownload3", new EntryInfo[] { new("OnLostRiverHintDownloaded1", null, true), new("OnLostRiverHintDownloaded2", null, true)}),
            new("Emperor_Telepathic_Contact1", new EntryInfo[] { new("OnEmperorFirstTelepathy", null, true)}),
            new("Goal_SecondarySystems", new EntryInfo[] { new("OnLifepodRepaired1", null, true), new("OnLifepodRepaired2", null, true)}),
            new("Goal_JellyCaveEntrance", new EntryInfo[] { new("OnJellyHintGiven", null, true)}),
            new("AdviseSelfScan", new EntryInfo[] { new("OnAdviseSelfScan", null, true)})
        };

        public struct StoryGoalTodoEntry
        {
            public string key;
            public EntryInfo[] entryInfos;

            public StoryGoalTodoEntry(string key, EntryInfo[] entryInfos)
            {
                this.key = key;
                this.entryInfos = entryInfos;
            }
        }

        public struct EntryInfo
        {
            public string entry;
            public string completeKey;
            public bool localized;

            public EntryInfo(string entry, string completeKey, bool localized)
            {
                this.entry = entry;
                this.completeKey = completeKey;
                this.localized = localized;
            }
        }
    }
}
