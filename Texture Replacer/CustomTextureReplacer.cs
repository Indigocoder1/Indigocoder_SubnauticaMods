using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using static TextureReplacer.Main;
using BepInEx;
using System.IO;
using Newtonsoft.Json;

namespace TextureReplacer
{
    public static class CustomTextureReplacer
    {
        private static string folderFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer");
        private static string configFilePath = Path.Combine(folderFilePath, "ExampleTextureConfig.json");
        private static List<TexturePatchConfigData> textureConfigs;

        internal static void Initialize()
        {
            CraftData.PreparePrefabIDCache();
            if(!Directory.Exists(folderFilePath))
            {
                Directory.CreateDirectory(folderFilePath);
            }

            textureConfigs = SaveManager<TexturePatchConfigData>.LoadJsons(folderFilePath);
            if (textureConfigs.Count == 0)
            {
                SaveExampleData();
                textureConfigs = SaveManager<TexturePatchConfigData>.LoadJsons(folderFilePath);
            }

            LoadAllTextures();
        }

        private static void LoadAllTextures()
        {
            for (int i = 0; i < textureConfigs.Count; i++)
            {
                TexturePatchConfigData configData = textureConfigs[i];

                if (configData == null)
                {
                    continue;
                }

                bool flag1 = configData.prefabClassID == "Intentionally blank" || string.IsNullOrEmpty(configData.prefabClassID);
                bool flag2 = configData.rendererHierarchyPath == "Intentionally blank" || string.IsNullOrEmpty(configData.rendererHierarchyPath);

                if (flag1 || flag2)
                {
                    if(Main.WriteLogs.Value)
                    {
                        Main.logger.LogInfo($"Skipping config {configData.configName} because it contains example data!");
                    }
                    continue;
                }

                CoroutineHost.StartCoroutine(InitializeTexture(configData));
            }
        }

        private static IEnumerator InitializeTexture(TexturePatchConfigData configData)
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
                if(rendererTransform == null)
                {
                    Main.logger.LogError($"There is no object at the hierarchy path '{configData.rendererHierarchyPath}'! Aborting texture load.");
                    yield break;
                }
                rendererTransform.TryGetComponent<Renderer>(out targetRenderer);

                if (targetRenderer == null && !Main.customTextureNames.ContainsKey(configData.textureName))
                {
                    Main.logger.LogError("Target renderer was null! Aborting texture load.");
                    yield break;
                }

                replacer.AddTextureData(configData);

                if (Main.WriteLogs.Value)
                {
                    Main.logger.LogInfo($"Loading of config {configData.configName} complete");
                }
            }
        }

        private static void SaveExampleData()
        {
            List<TexturePatchConfigData> configDatas = new List<TexturePatchConfigData>
            {
                new TexturePatchConfigData
                (
                    configName: "Example Config Name",
                    materialIndex: 0,
                    fileName: "Replacement texture file name goes here",
                    isVariation: false,
                    variationChance: -1f,
                    prefabClassID: "Intentionally blank",
                    rendererHierarchyPath: "Intentionally blank",
                    textureName: "_MainTex",
                    linkedConfigNames: new List<string>
                    {
                        "Example name 1",
                        "Example name 2"
                    }
                )
            };

            SaveManager<TexturePatchConfigData>.SaveToJson(configDatas, configFilePath, folderFilePath);
        }

        /// <summary>
        /// Takes a <see cref="TexturePatchConfigData"/> and adds it to the config list and initializes it
        /// </summary>
        public static void AddConfig(TexturePatchConfigData configData)
        {
            textureConfigs.Add(configData);
            CoroutineHost.StartCoroutine(InitializeTexture(configData));
        }

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

            internal TexturePatchConfigData(ConfigInfo configInfo)
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
