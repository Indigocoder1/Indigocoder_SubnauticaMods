using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Json.Converters;
using Newtonsoft.Json;
using System;
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
        //public static HashSet<TechType> defaultTech = new();
        public static CustomCraftCacheData cacheData { get; private set; } = new();

        private void Awake()
        {
            logger = Logger;

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "customcraftgui"));
            harmony.PatchAll();

            cacheData = TryLoadCachedTech();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private CustomCraftCacheData TryLoadCachedTech()
        {
            if(!File.Exists(customCraftGUICachePath))
            {
                return null;
            }

            string data = File.ReadAllText(customCraftGUICachePath);
            CustomCraftCacheData cachedDefaultTech = null;
            try
            {
                cachedDefaultTech = JsonConvert.DeserializeObject<CustomCraftCacheData>(data, new CustomEnumConverter());
            }
            catch(Exception e)
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

            if(cacheData.defaultTech == null && cacheData.analysisTech == null)
            {
                return;
            }

            var defaultTechJson = JsonConvert.SerializeObject(cacheData, Formatting.Indented, new CustomEnumConverter());
            File.WriteAllText(customCraftGUICachePath, defaultTechJson);
        }
    }

    public class CustomCraftCacheData
    {
        public HashSet<TechType> defaultTech;
        public List<KnownTech.AnalysisTech> analysisTech;

        public CustomCraftCacheData(HashSet<TechType> defaultTech, List<KnownTech.AnalysisTech> analysisTech)
        {
            this.defaultTech = defaultTech;
            this.analysisTech = analysisTech;
        }

        public CustomCraftCacheData()
        {

        }
    }
}
