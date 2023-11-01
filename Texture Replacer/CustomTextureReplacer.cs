using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using static TextureReplacer.Main;
using BepInEx;
using System.IO;

namespace TextureReplacer
{
    internal static class CustomTextureReplacer
    {
        private static string folderFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer");
        private static string configFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer/ExampleTextureConfig.json");
        private static List<TexturePatchConfigData> textureConfigs;

        public static void Initialize()
        {
            textureConfigs = SaveManager<Main.TexturePatchConfigData>.LoadJsons(folderFilePath);
            if (textureConfigs == null)
            {
                SaveExampleData();
                textureConfigs = SaveManager<Main.TexturePatchConfigData>.LoadJsons(folderFilePath);
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
                    if(Main.logs.Value)
                    {
                        Main.logger.LogDebug($"Skipping config {configData.configName} because it contains example data!");
                    }
                    continue;
                }

                CoroutineHost.StartCoroutine(InitializeTexture(configData));
            }
        }

        private static IEnumerator InitializeTexture(TexturePatchConfigData configData)
        {
            if (Main.logs.Value)
            {
                Main.logger.LogDebug($"Loading config {configData.configName}");
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
                    if(Main.logs.Value)
                    {
                        Main.logger.LogError($"There is no object at the hierarchy path '{configData.rendererHierarchyPath}'! Aborting texture load.");
                    }
                    yield break;
                }
                rendererTransform.TryGetComponent<Renderer>(out targetRenderer);

                if (targetRenderer == null && !Main.customTextureNames.ContainsKey(configData.textureName))
                {
                    if (Main.logs.Value)
                    {
                        Main.logger.LogError("Target renderer was null! Aborting texture load.");
                    }
                    yield break;
                }

                replacer.AddTextureData(configData);

                if (Main.logs.Value)
                {
                    Main.logger.LogDebug($"Loading of config {configData.configName} complete");
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

            SaveManager<Main.TexturePatchConfigData>.SaveToJson(configDatas, configFilePath, folderFilePath);
        }
    }
}
