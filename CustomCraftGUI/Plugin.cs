using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Json.Converters;
using Nautilus.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomCraftGUI
{
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.CustomCraftGUI";
        private const string pluginName = "Custom Craft GUI";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle assetBundle { get; private set; }

        private readonly static string customCraftGUICachePath = Path.Combine(Paths.ConfigPath, "CCGUI_DefaultTechCache.json");
        public static CustomCraftCacheData cacheData { get; private set; } = new();

        private void Awake()
        {
            logger = Logger;

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "customcraftgui"));
            harmony.PatchAll();            

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private IEnumerator Start()
        {
            //Wait until most mods are loaded so that JsonConvert has all the necessary types
            yield return new WaitUntil(() => MaterialUtils.IsReady);
            yield return new WaitForSeconds(2f);

            cacheData = TryLoadCachedTech();
        }

        private CustomCraftCacheData TryLoadCachedTech()
        {
            if (!File.Exists(customCraftGUICachePath))
            {
                return new();
            }

            string data = File.ReadAllText(customCraftGUICachePath);
            CustomCraftCacheData cachedDefaultTech = new();
            try
            {
                cachedDefaultTech = JsonConvert.DeserializeObject<CustomCraftCacheData>(data, new CustomEnumConverter());
            }
            catch (Exception e)
            {
                logger.LogError($"Error loading cached tech from {customCraftGUICachePath} | {e.Message}");
            }

            return cachedDefaultTech;
        }

        public static void CacheDefaultTech()
        {
            if (File.Exists(customCraftGUICachePath))
            {
                return;
            }

            if (cacheData.defaultTech == null && cacheData.analysisTech == null)
            {
                return;
            }

            var defaultTechJson = JsonConvert.SerializeObject(cacheData, Formatting.Indented, new CustomEnumConverter());
            File.WriteAllText(customCraftGUICachePath, defaultTechJson);
        }

        public class CustomCraftCacheData
        {
            public HashSet<TechType> defaultTech;
            public List<SlimAnalysisTech> analysisTech;

            public CustomCraftCacheData(HashSet<TechType> defaultTech, List<SlimAnalysisTech> analysisTech)
            {
                this.defaultTech = defaultTech;
                this.analysisTech = analysisTech;
            }

            public CustomCraftCacheData()
            {

            }
        }

        public class SlimAnalysisTech
        {
            public TechType techType;
            public List<TechType> unlockTechTypes;

            public SlimAnalysisTech(TechType techType, List<TechType> unlockTechTypes)
            {
                this.techType = techType;
                this.unlockTechTypes = unlockTechTypes;
            }
        }
    }
}
