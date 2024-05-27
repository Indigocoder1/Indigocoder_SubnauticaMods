﻿using BepInEx;
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

        internal static Dictionary<string, string[]> StoryGoalTodoEntries { get; } = new()
        {
            { "Trigger_PDAIntroEnd", new[] { "Craft a Scanner Tool", "Repair the Lifepod's secondary systems", "Repair the Lifepod radio" } },
            { "Story_AuroraWarning4", new[] { "Repair the Aurora" } },
            { "OnPlayRadioBloodKelp29", new[] { "Investigate lifepod 2" } },
            { "OnPlayRadioGrassy25", new[] { "Investigate lifepod 3" } },
            { "OnPlayRadioRadiationSuit", new[] { "Investigate lifepod 4" } },
            { "OnPlayRadioShallows22", new[] { "Investigate lifepod 6" } },
            { "OnPlayRadioKelp28", new[] { "Investigate lifepod 7" } },
            { "OnPlayRadioKoosh26", new[] { "Investigate lifepod 12" } },
            { "OnPlayRadioGrassy21", new[] { "Investigate lifepod 17" } },
            { "OnPlayRadioSecondOfficer", new[] { "Investigate Officer Keen's lifepod (Lifepod 19)" } },
            { "OnPlayRadioCaptainsQuartersCode", new[] { "Investigate the Aurora's captain's quarters" } },
            { "OnPlayRadioSunbeam4", new[] { "Visit the Sunbeam landing site" } },
            { "Precursor_Gun_DataDownload3", new[] { "Explore for the Disease Research Facility", "Explore for the Thermal Power Facility" } },
        };
    }
}
