using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using static TextureReplacer.Main;
using BepInEx;
using System.IO;
using static TextureReplacer.LifepodTextureReplacer;

namespace TextureReplacer
{
    internal static class CustomTextureReplacer
    {
        private static string folderFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer");
        private static string configFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer/CustomTextureConfig.json");
        private static List<TexturePatchConfigData> textureConfigs;

        public static void Initialize()
        {
            textureConfigs = SaveManager.LoadFromJson(configFilePath);
            if (textureConfigs == null)
            {
                SaveExampleData();
                textureConfigs = SaveManager.LoadFromJson(configFilePath);
            }

            LoadAllTextures();
        }

        private static void LoadAllTextures()
        {
            List<TexturePatchConfigData> linkedTextureConfigs = new List<TexturePatchConfigData>();

            for (int i = 0; i < textureConfigs.Count; i++)
            {
                TexturePatchConfigData configData = textureConfigs[i];
                if(configData == null)
                {
                    return;
                }

                bool flag1 = configData.prefabClassID == "Intentionally blank" || string.IsNullOrEmpty(configData.prefabClassID);
                bool flag2 = configData.rendererHierarchyPath == "Intentionally blank" || string.IsNullOrEmpty(configData.rendererHierarchyPath);

                logger.LogInfo($"Is blank flag 1 = {flag1} | Is blank flag 2 = {flag2}");

                if (flag1 || flag2)
                {
                    return;
                }

                configData.variationChance = Mathf.Clamp01(configData.variationChance);

                if (configData.isVariation)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= configData.variationChance || configData.variationAccepted)
                    {
                        Main.logger.LogInfo($"Variation from {configData.fileName} accepted");

                        if(configData.linkedConfigNames.Count > 0)
                        {
                            EnsureLinkedConfigs(configData);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                CoroutineHost.StartCoroutine(InitializeTexture(configData.materialIndex, configData.fileName, configData.prefabClassID, configData.rendererHierarchyPath));
            }
        }

        private static void EnsureLinkedConfigs(TexturePatchConfigData configData)
        {
            foreach (TexturePatchConfigData item in textureConfigs)
            {
                if(!item.isVariation || item.variationChance == 1)
                {
                    continue;
                }
                
                if(item.linkedConfigNames.Count == 0)
                {
                    continue;
                }

                if(item.linkedConfigNames.Contains(configData.configName))
                {
                    item.variationAccepted = true;
                }
            }
        }

        private static IEnumerator InitializeTexture(int materialIndex, string textureName, string classID, string hierchyPath)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(classID);

            yield return request;

            if (request.TryGetPrefab(out GameObject prefab))
            {
                TextureReplacerHelper replacer = prefab.AddComponent<TextureReplacerHelper>();

                Renderer targetRenderer = prefab.transform.Find(hierchyPath).GetComponent<Renderer>();

                if (targetRenderer == null)
                {
                    Main.logger.LogError($"Target renderer was null!");
                    yield break;
                }

                replacer.ReplaceTexture(targetRenderer.materials[materialIndex], textureName);
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
                    linkedConfigNames: new List<string>
                    {
                        "Example name 1",
                        "Example name 2"
                    }
                )
            };

            SaveManager.SaveToJson(configDatas, configFilePath, folderFilePath);
        }
    }
}
