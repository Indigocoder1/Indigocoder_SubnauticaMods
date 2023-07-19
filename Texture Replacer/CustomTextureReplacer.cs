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
            }

            LoadAllTextures();
        }

        private static void LoadAllTextures()
        {
            for (int i = 0; i < textureConfigs.Count; i++)
            {
                TexturePatchConfigData configData = textureConfigs[i];
                if(configData == null)
                {
                    return;
                }


                bool flag1 = configData.prefabClassID == "Intentionally blank" || string.IsNullOrEmpty(configData.prefabClassID);
                bool flag2 = configData.rendererHierchyPath == "Intentionally blank" || string.IsNullOrEmpty(configData.rendererHierchyPath);

                logger.LogInfo($"Flag1 = {flag1} | Flag2 = {flag2}");

                if (flag1 || flag2)
                {
                    return;
                }

                configData.variationChance = Mathf.Clamp01(configData.variationChance);

                if (configData.isVariation)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= configData.variationChance)
                    {
                        Main.logger.LogInfo($"Variation from {configData.fileName} accepted");
                    }
                    else
                    {
                        continue;
                    }
                }

                CoroutineHost.StartCoroutine(InitializeTexture(configData.materialIndex, configData.fileName, configData.prefabClassID, configData.rendererHierchyPath));
            }
        }

        private static IEnumerator InitializeTexture(int materialIndex, string textureName, string classID, string hierchyPath)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(classID);

            if (request == null)
            {
                yield break;
            }

            if (request.TryGetPrefab(out GameObject prefab))
            {
                TextureReplacerHelper replacer = prefab.AddComponent<TextureReplacerHelper>();

                Renderer targetRenderer = prefab.transform.Find(hierchyPath).GetComponent<Renderer>();

                if (targetRenderer == null)
                {
                    Main.logger.LogError($"Target renderer was null!");
                    yield return null;
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
                    materialIndex: 0,
                    fileName: "Replacement texture file name goes here",
                    isVariation: false,
                    variationChance: -1f,
                    prefabClassID: "Intentionally blank",
                    rendererHierchyPath: "Intentionally blank"
                )
            };

            SaveManager.SaveToJson(configDatas, configFilePath, folderFilePath);
        }
    }
}
