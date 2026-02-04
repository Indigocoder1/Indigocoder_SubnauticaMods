using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nautilus.Handlers;

namespace TextureReplacer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TextureReplacer";
        private const string pluginName = "Texture Replacer";
        private const string versionString = "1.2.3";

        public static ManualLogSource logger;
        public static string AssetFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static ConfigEntry<bool> WriteLogs;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
        {
            logger = Logger;

            WriteLogs = Config.Bind("Enable logging for textures", "Write Logs", false, 
                new ConfigDescription("Warning: When using many textures, there will be lots of logs\n(Errors will always log)"));

            harmony.PatchAll();
            
            new TextureReplacerOptions();
            WaitScreenHandler.RegisterAsyncLoadTask("Texture replacer", LoadConfigs, "Loading textures");

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private IEnumerator LoadConfigs(WaitScreenHandler.WaitScreenTask task)
        {
            if (CustomTextureReplacer.textureConfigs.Count == 0) yield break;
            
            if (CustomTextureReplacer.GetLoadProgress() >= 1) yield break;
            
            while (CustomTextureReplacer.GetLoadProgress() < 1)
            {
                yield return CustomTextureReplacer.Initialize();
                var loadProgress = (100 * CustomTextureReplacer.GetLoadProgress()).ToString("F0");
                task.Status = $"Loading textures ({loadProgress}%)";
            }
        }

        public struct LegacyConfigInfo
        {
            public int materialIndex;
            public string fileName;
            public string prefabClassID;
            public string rendererHierchyPath;

            public bool isVariation;
            public float variationChance;
            public List<string> linkedConfigNames;

            public LegacyConfigInfo(int materialIndex, string fileName, string prefabClassID, string rendererHierchyPath, bool isVariation, float variationChance, List<string> linkedConfigNames)
            {
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.prefabClassID = prefabClassID;
                this.rendererHierchyPath = rendererHierchyPath;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
                this.linkedConfigNames = linkedConfigNames;
            }
        }

        public static Dictionary<string, TextureType> customTextureNames = new Dictionary<string, TextureType>
        {
            { "_EmissionMap", TextureType.Emission},
            { "_EmissionColor", TextureType.LightColor},
            { "_Color", TextureType.Color }
        };

        public static Dictionary<TextureType, float> textureNameValueDefaults = new Dictionary<TextureType, float>
        {
            { TextureType.Emission, 0.6f },
            { TextureType.LightColor, 2.6f },
            { TextureType.Value, 1.4f }
        };

        public enum TextureType
        {
            Emission,
            LightColor,
            Color,
            Value
        }
    }
}
