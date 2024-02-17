using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TextureReplacer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TextureReplacer";
        private const string pluginName = "Texture Replacer";
        private const string versionString = "1.1.5";

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

            //LifepodTextureReplacer.Initialize();
            CustomTextureReplacer.Initialize();
            new TextureReplacerOptions();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        public struct ConfigInfo
        {
            public int materialIndex;
            public string fileName;
            public string prefabClassID;
            public string rendererHierchyPath;

            public bool isVariation;
            public float variationChance;
            public List<string> linkedConfigNames;

            public ConfigInfo(int materialIndex, string fileName, string prefabClassID, string rendererHierchyPath, bool isVariation, float variationChance, List<string> linkedConfigNames)
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
