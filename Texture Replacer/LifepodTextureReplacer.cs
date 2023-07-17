﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace TextureReplacer
{
    internal static class LifepodTextureReplacer
    {
        public static void LoadAllTextures()
        {
            for (int i = 0; i < Main.textureConfig.textureConfigs.Count; i++)
            {
                Main.LifepodConfigItem configItem = Main.textureConfig.textureConfigs[i];
                configItem.variationChance = Mathf.Clamp01(configItem.variationChance);
                bool variationAccepted = false;

                if (configItem.isVariation)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= configItem.variationChance)
                    {
                        variationAccepted = true;
                    }
                }

                if (!variationAccepted)
                {
                    CoroutineHost.StartCoroutine(InitializeTexture(configItem.materialIndex, configItem.fileName, LifepodFromIndex[configItem.lifepodNumberIndex]));
                }
                else
                {
                    HandleAlternateTexture(configItem);
                }
            }
        }

        private static void HandleAlternateTexture(Main.LifepodConfigItem alternateConfig)
        {
            Main.logger.LogInfo($"Variation from {alternateConfig.fileName} accepted");

            for (int i = 0; i < Main.textureConfig.textureConfigs.Count; i++)
            {
                if (TargetingSameMaterial(alternateConfig, Main.textureConfig.textureConfigs[i]))
                {
                    CoroutineHost.StartCoroutine(InitializeTexture(alternateConfig.materialIndex, alternateConfig.fileName, LifepodFromIndex[alternateConfig.lifepodNumberIndex]));
                }
            }
        }

        private static bool TargetingSameMaterial(Main.LifepodConfigItem config1, Main.LifepodConfigItem config2)
        {
            bool check1 = config1.materialIndex == config2.materialIndex;
            bool check2 = config1.lifepodNumberIndex == config2.lifepodNumberIndex;

            if (check1 && check2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static IEnumerator InitializeTexture(int materialIndex, string textureName, LifepodNumber lifepodNumber)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(LifepodClassIDs[lifepodNumber]);

            yield return request;

            if (request.TryGetPrefab(out GameObject prefab))
            {
                Texture_Replacer replacer = prefab.AddComponent<Texture_Replacer>();

                Renderer targetRenderer = prefab.transform.Find(ExternalRendererHierchyPaths[lifepodNumber]).GetComponent<Renderer>();

                if (targetRenderer == null)
                {
                    Main.logger.LogError($"Target renderer was null!");
                    yield return null;
                }

                replacer.ReplaceTexture(targetRenderer.materials[materialIndex], textureName);
            }
        }

        public enum LifepodNumber
        {
            Lifepod2,
            Lifepod3,
            Lifepod4,
            Lifepod6,
            Lifepod7,
            Lifepod12,
            Lifepod13,
            Lifepod17,
            Lifepod19
        }

        private static readonly Dictionary<LifepodNumber, string> ExternalRendererHierchyPaths = new Dictionary<LifepodNumber, string>
        {
            { LifepodNumber.Lifepod2, "life_pod_exploded_02/life_pod"},
            { LifepodNumber.Lifepod3, "life_pod_exploded_02_01/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod4, "life_pod_exploded_01/life_pod"},
            { LifepodNumber.Lifepod6, "life_pod_exploded_02_02/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod7, "life_pod_exploded_02/life_pod"},
            { LifepodNumber.Lifepod12, "life_pod_exploded_02_03/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod13, "life_pod_exploded_02_02/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod17, "life_pod_exploded_02_03/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod19, "life_pod_exploded_02_01/exterior/life_pod_damaged"},
        };
        private static readonly Dictionary<LifepodNumber, string> LifepodClassIDs = new Dictionary<LifepodNumber, string>
        {
            { LifepodNumber.Lifepod2, "66cc5a83-142b-4d8d-8d16-2d6e960f59c3"},
            { LifepodNumber.Lifepod3, "2aa237f6-2103-4a78-aaa7-104216551f0a"},
            { LifepodNumber.Lifepod4, "f2b9fe45-39d6-4307-b1e0-143eb1937d6e"},
            { LifepodNumber.Lifepod6, "85ae70e0-176c-4de6-8c4d-48c4f504cc79"},
            { LifepodNumber.Lifepod7, "d3b9095f-fcac-46de-83f7-762e3275e837"},
            { LifepodNumber.Lifepod12, "00891fdf-7264-4c55-b569-732cdcded701"},
            { LifepodNumber.Lifepod13, "00037e80-3037-48cf-b769-dc97c761e5f6"},
            { LifepodNumber.Lifepod17, "56b5ed17-2bff-4f7e-aba0-275b6a2398f9"},
            { LifepodNumber.Lifepod19, "3894aeaf-e1f9-426a-9249-6a4968ac2d8b"},
        };
        private static readonly Dictionary<int, LifepodNumber> LifepodFromIndex = new Dictionary<int, LifepodNumber>
        {
            { 0, LifepodNumber.Lifepod2 },
            { 1, LifepodNumber.Lifepod3 },
            { 2, LifepodNumber.Lifepod4 },
            { 3, LifepodNumber.Lifepod6 },
            { 4, LifepodNumber.Lifepod7 },
            { 5, LifepodNumber.Lifepod12 },
            { 6, LifepodNumber.Lifepod13 },
            { 7, LifepodNumber.Lifepod17 },
            { 8, LifepodNumber.Lifepod19 }
        };
        private static readonly Dictionary<LifepodNumber, int> IndexFromLifepod = new Dictionary<LifepodNumber, int>
        {
            { LifepodNumber.Lifepod2, 0 },
            { LifepodNumber.Lifepod3, 1 },
            { LifepodNumber.Lifepod4, 2 },
            { LifepodNumber.Lifepod6, 3 },
            { LifepodNumber.Lifepod7, 4 },
            { LifepodNumber.Lifepod12, 5 },
            { LifepodNumber.Lifepod13, 6 },
            { LifepodNumber.Lifepod17, 7 },
            { LifepodNumber.Lifepod19, 8 }
        };
    }
}