using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using static TextureReplacer.Main;
using BepInEx;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;
using TextureReplacer.Saving;
using Nautilus.Utility;
using UnityStandardAssets.ImageEffects;

namespace TextureReplacer
{
    public static class CustomTextureReplacer
    {
        public static List<ConfigInfo> queuedPlayerConfigs = new();
        public static List<ConfigInfo> queuedCyclopsConfigs = new();
        public static List<ConfigInfo> textureConfigs { get; private set; } = new();

        private static string folderFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer");
        private static string configFilePath = Path.Combine(folderFilePath, "ExampleTextureConfig.json");

        internal static void Initialize()
        {
            CraftData.PreparePrefabIDCache();
            if (!Directory.Exists(folderFilePath))
            {
                Directory.CreateDirectory(folderFilePath);
            }

            ConvertLegacyConfigs();

            textureConfigs.AddRange(SaveManager<ConfigInfo>.LoadJsons(folderFilePath));

            if (textureConfigs.Count == 0)
            {
                SaveExampleData();
                textureConfigs.AddRange(SaveManager<ConfigInfo>.LoadJsons(folderFilePath));
            }

            LoadAllTextures();
        }

        private static void ConvertLegacyConfigs()
        {
            var legacyConfigs = SaveManager<TexturePatchConfigData>.LoadJsons(folderFilePath);
            List<ConfigInfo> configInfoConversions = new();

            foreach (var legacyConfig in legacyConfigs)
            {
                ConfigInfo info = new(legacyConfig.configName, legacyConfig.prefabClassID, legacyConfig.rendererHierarchyPath,
                    legacyConfig.isVariation, legacyConfig.variationChance, legacyConfig.linkedConfigNames, new());

                if (!configInfoConversions.Any(i => i.prefabClassID == legacyConfig.prefabClassID && i.rendererHierarchyPath == legacyConfig.rendererHierarchyPath))
                {
                    configInfoConversions.Add(info);
                }

                string texName = legacyConfig.textureName;
                if (texName.Contains("-"))
                {
                    texName = texName.Split(new char[] { '-' }, 2)[0];
                }

                TextureEditType type = legacyConfig.textureName.Contains("_EmissionColor") ? TextureEditType.Light : TextureEditType.Texture;
                string data = legacyConfig.fileName;
                if (type == TextureEditType.Light)
                {
                    Texture2D tex = ImageUtils.LoadTextureFromFile(Main.AssetFolderPath + $"/{legacyConfig.fileName}");
                    Color32 col = AverageColorFromTexture(tex);
                    data = $"{col.r / 255f},{col.g / 255f},{col.b / 255f},{col.a / 255f}";
                }

                ConfigInfo retrievedInfo = configInfoConversions.First(i => i.prefabClassID == legacyConfig.prefabClassID && i.rendererHierarchyPath == legacyConfig.rendererHierarchyPath);
                retrievedInfo.textureEdits.Add(new(legacyConfig.materialIndex, type, legacyConfig.textureName, data));

                if (legacyConfig.textureName.Contains("_EmissionMap"))
                {
                    retrievedInfo.textureEdits.Add(new(legacyConfig.materialIndex, TextureEditType.Float, "_GlowStrength", "0"));
                    retrievedInfo.textureEdits.Add(new(legacyConfig.materialIndex, TextureEditType.Float, "_GlowStrengthNight", "0"));
                    retrievedInfo.textureEdits.Add(new(legacyConfig.materialIndex, TextureEditType.Float, "_EmissionLM", "0.6"));
                    retrievedInfo.textureEdits.Add(new(legacyConfig.materialIndex, TextureEditType.Float, "_EmissionLMNight", "0.6"));
                }
            }

            foreach (var texEdit in configInfoConversions)
            {
                textureConfigs.Add(texEdit);
            }
        }

        private static void LoadAllTextures()
        {
            for (int i = 0; i < textureConfigs.Count; i++)
            {
                ConfigInfo configData = textureConfigs[i];

                bool flag1 = configData.prefabClassID == "Intentionally blank" || string.IsNullOrEmpty(configData.prefabClassID);
                bool flag2 = configData.rendererHierarchyPath == "Intentionally blank" || string.IsNullOrEmpty(configData.rendererHierarchyPath);

                if (flag1 || flag2)
                {
                    if (Main.WriteLogs.Value)
                    {
                        Main.logger.LogInfo($"Skipping config {configData.configName} because it contains example data!");
                    }
                    continue;
                }

                if (configData.prefabClassID.ToLower() == "player")
                {
                    queuedPlayerConfigs.Add(configData);

                    if (Main.WriteLogs.Value)
                    {
                        Main.logger.LogInfo($"Adding of queued config {configData.configName} complete (Config for player GO)");
                    }

                    continue;
                }

                if (configData.prefabClassID.ToLower() == "cyclops")
                {
                    queuedCyclopsConfigs.Add(configData);

                    if (Main.WriteLogs.Value)
                    {
                        Main.logger.LogInfo($"Adding of queued config {configData.configName} complete (Config for Cyclops GO)");
                    }

                    continue;
                }

                CoroutineHost.StartCoroutine(InitializeTexture(configData));
            }
        }

        private static IEnumerator InitializeTexture(ConfigInfo configData)
        {
            if (Main.WriteLogs.Value)
            {
                Main.logger.LogInfo($"Loading config {configData.configName}");
            }

            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(configData.prefabClassID);

            yield return request;

            if (request.TryGetPrefab(out GameObject prefab))
            {
                TextureReplacerHelper replacer = prefab.EnsureComponent<TextureReplacerHelper>();

                Renderer targetRenderer = null;
                Transform rendererTransform = prefab.transform.Find(configData.rendererHierarchyPath);
                if (rendererTransform == null)
                {
                    Main.logger.LogError($"There is no object at the hierarchy path '{configData.rendererHierarchyPath}' on {prefab}! Aborting texture load.");
                    yield break;
                }
                rendererTransform.TryGetComponent<Renderer>(out targetRenderer);

                replacer.AddTextureData(configData);

                if (Main.WriteLogs.Value)
                {
                    Main.logger.LogInfo($"Loading of config {configData.configName} complete");
                }
            }
        }

        private static void SaveExampleData()
        {
            List<ConfigInfo> configDatas = new List<ConfigInfo>
            {
                new ConfigInfo
                (
                    configName: "Example Config Name",
                    isVariation: false,
                    variationChance: -1f,
                    prefabClassID: "Intentionally blank",
                    rendererHierarchyPath: "Intentionally blank",
                    linkedConfigNames: new List<string>
                    {
                        "Example name 1",
                        "Example name 2"
                    },
                    textureEdits: new List<ConfigInfo.TextureEdit>
                    {
                        new(0, TextureEditType.Texture, "_MainTex", "textureName.fileExtension")
                    }
                )
            };

            SaveManager<ConfigInfo>.SaveToJson(configDatas, configFilePath, folderFilePath);
        }

        private static Color32 AverageColorFromTexture(Texture2D tex)
        {
            Color32[] texColors = tex.GetPixels32();

            int total = texColors.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = 0; i < total; i++)
            {
                r += texColors[i].r;
                g += texColors[i].g;
                b += texColors[i].b;
            }

            Color32 color = new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
            if (Main.WriteLogs.Value)
            {
                Main.logger.LogInfo($"Average color for texture \"{tex.name}\" is {color}");
            }

            return color;
        }

        public class ConfigInfo
        {
            [SerializeField] public string configName;
            [SerializeField] public string prefabClassID;
            [SerializeField] public string rendererHierarchyPath;
            [SerializeField] public bool isVariation;
            [SerializeField] public float variationChance;
            [SerializeField] public List<string> linkedConfigNames;
            [SerializeField] public List<TextureEdit> textureEdits;

            [JsonIgnore]
            [SerializeField] public bool variationAccepted;

            public ConfigInfo() { }

            [JsonConstructor]
            public ConfigInfo(string configName, string prefabClassID, string rendererHierarchyPath, bool isVariation,
                float variationChance, List<string> linkedConfigNames, List<TextureEdit> textureEdits)
            {
                this.configName = configName;
                this.prefabClassID = prefabClassID;
                this.rendererHierarchyPath = rendererHierarchyPath;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
                this.linkedConfigNames = linkedConfigNames;
                this.textureEdits = textureEdits;
            }

            public class TextureEdit
            {
                [SerializeField] public int materialIndex;
                [SerializeField] public TextureEditType editType;
                [SerializeField] public string propertyName;
                [SerializeField] public string data;

                [JsonIgnore]
                [SerializeField] public Texture2D cachedTexture;

                public TextureEdit(int materialIndex, TextureEditType editType, string propertyName, string data)
                {
                    this.materialIndex = materialIndex;
                    this.editType = editType;
                    this.propertyName = propertyName;
                    this.data = data;
                }
            }
        }

        public enum TextureEditType
        {
            Texture,
            Sprite,
            Color,
            Float,
            Vector,
            Keyword,
            Light
        }

        /// <summary>
        /// Takes a <see cref="ConfigInfo"/> and adds it to the config list and initializes it
        /// </summary>
        public static void AddConfig(ConfigInfo configData)
        {
            textureConfigs.Add(configData);
            CoroutineHost.StartCoroutine(InitializeTexture(configData));
        }

        [Obsolete("Use the override that takes a ConfigInfo instead")]
        public static void AddConfig(TexturePatchConfigData configData)
        {
            ConfigInfo info = new(configData.configName, configData.prefabClassID,
                configData.rendererHierarchyPath, configData.isVariation, configData.variationChance, configData.linkedConfigNames,
                new List<ConfigInfo.TextureEdit> { new ConfigInfo.TextureEdit(configData.materialIndex,
                TextureEditType.Texture, configData.textureName, configData.fileName) });
            textureConfigs.Add(info);
            CoroutineHost.StartCoroutine(InitializeTexture(info));
        }

        [Obsolete("Use ConfigInfo instead")]
        public class TexturePatchConfigData
        {
            public string configName;
            public int materialIndex;
            public string fileName;
            public string prefabClassID;
            public string rendererHierarchyPath;
            public string textureName;

            public bool isVariation;
            public float variationChance;
            public List<string> linkedConfigNames;
            [JsonIgnore]
            public bool variationAccepted;

            [JsonConstructor]
            public TexturePatchConfigData(string configName, int materialIndex, string fileName, bool isVariation, float variationChance,
                string prefabClassID, string rendererHierarchyPath, string textureName, List<string> linkedConfigNames)
            {
                this.configName = configName;
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.prefabClassID = prefabClassID;
                this.rendererHierarchyPath = rendererHierarchyPath;
                this.textureName = textureName;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
                this.linkedConfigNames = linkedConfigNames;
            }

            internal TexturePatchConfigData(LegacyConfigInfo configInfo)
            {
                this.materialIndex = configInfo.materialIndex;
                this.fileName = configInfo.fileName;
                this.prefabClassID = configInfo.prefabClassID;
                this.rendererHierarchyPath = configInfo.rendererHierchyPath;
                this.isVariation = configInfo.isVariation;
                this.variationChance = configInfo.variationChance;
                this.linkedConfigNames = configInfo.linkedConfigNames;
            }
        }
    }
}
