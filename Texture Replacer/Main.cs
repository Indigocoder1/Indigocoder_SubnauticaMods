using BepInEx;
using BepInEx.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextureReplacer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TextureReplacer";
        private const string pluginName = "Texture Replacer";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            LifepodTextureReplacer.Initialize();
            CustomTextureReplacer.Initialize();
        }

        public class TexturePatchConfigData
        {
            public string configName;
            public int materialIndex;
            public string fileName;
            public string prefabClassID;
            public string rendererHierarchyPath;

            public bool isVariation;
            public float variationChance;
            public List<string> linkedConfigNames;
            [JsonIgnore]
            public bool variationAccepted;

            [JsonConstructor]
            public TexturePatchConfigData(string configName, int materialIndex, string fileName, bool isVariation, float variationChance, string prefabClassID, string rendererHierarchyPath, List<string> linkedConfigNames)
            {
                this.configName = configName;
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.prefabClassID = prefabClassID;
                this.rendererHierarchyPath = rendererHierarchyPath;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
                this.linkedConfigNames = linkedConfigNames;
            }

            public TexturePatchConfigData(ConfigInfo configInfo)
            {
                this.materialIndex = configInfo.materialIndex;
                this.fileName = configInfo.fileName;
                this.prefabClassID = configInfo.prefabClassID;
                this.rendererHierarchyPath = configInfo.rendererHierchyPath;
                this.isVariation = configInfo.isVariation;
                this.variationChance = configInfo.variationChance;
            }
        }

        public struct ConfigInfo
        {
            public int materialIndex;
            public string fileName;
            public string prefabClassID;
            public string rendererHierchyPath;

            public bool isVariation;
            public float variationChance;

            public ConfigInfo(int materialIndex, string fileName, string prefabClassID, string rendererHierchyPath, bool isVariation, float variationChance)
            {
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.prefabClassID = prefabClassID;
                this.rendererHierchyPath = rendererHierchyPath;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
            }
        }
    }
}
