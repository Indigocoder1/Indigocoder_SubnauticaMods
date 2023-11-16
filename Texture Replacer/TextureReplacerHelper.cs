using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;
using IndigocoderLib;
using System.Collections;
using static TextureReplacer.Main;
using Random = UnityEngine.Random;
using System;

namespace TextureReplacer
{
    internal class TextureReplacerHelper : MonoBehaviour
    {
        private static Dictionary<string, List<TexturePatchConfigData>> configDatas;
        private static Dictionary<string, List<string>> filePaths;
        private static Dictionary<string, List<Texture2D>> textures;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            SetUpTextures();
        }

        public void AddTextureData(TexturePatchConfigData configData)
        {
            InitializeDictionaries();

            if (!configDatas.ContainsKey(transform.name))
            {
                configDatas.Add(transform.name, new List<TexturePatchConfigData>());
            }
            if (!filePaths.ContainsKey(transform.name))
            {
                filePaths.Add(transform.name, new List<string>());
            }
            if (!textures.ContainsKey(transform.name))
            {
                textures.Add(transform.name, new List<Texture2D>());
            }
            configDatas[transform.name].Add(configData);
            filePaths[transform.name].Add(AssetFolderPath + $"/{configData.fileName}");
            textures[transform.name].Add(ImageUtils.LoadTextureFromFile(AssetFolderPath + $"/{configData.fileName}"));
        }

        private void InitializeDictionaries()
        {
            if (configDatas == null)
            {
                configDatas = new Dictionary<string, List<TexturePatchConfigData>>();
            }

            if (filePaths == null)
            {
                filePaths = new Dictionary<string, List<string>>();
            }

            if (textures == null)
            {
                textures = new Dictionary<string, List<Texture2D>>();
            }
        }

        private void SetUpTextures()
        {
            string nameWithoutClone = Utilities.GetNameWithCloneRemoved(transform.name);

            for (int i = 0; i < configDatas[nameWithoutClone].Count; i++)
            {
                TexturePatchConfigData configData = configDatas[nameWithoutClone][i];

                if (Main.WriteLogs.Value)
                {
                    Main.logger.LogInfo($"Loading textures on {transform.name} from {configData.configName}");
                }

                if (configData.variationAccepted || !configData.isVariation || Random.Range(0f, 1f) <= configData.variationChance)
                {
                    Texture2D texture = textures[nameWithoutClone][i];

                    Renderer targetRenderer = transform.Find(configData.rendererHierarchyPath).GetComponent<Renderer>();
                    Material material = null;
                    if (targetRenderer != null)
                    {
                        material = targetRenderer.materials[configData.materialIndex];
                    }

                    if (configData.textureName.Contains("-"))
                    {
                        string[] split = configData.textureName.Split(new[] { '-' }, 2);
                        float extractedValue = textureNameValueDefaults[customTextureNames[split[0]]];
                        if (customTextureNames.ContainsKey(split[0]))
                        {
                            try
                            {
                                extractedValue = float.Parse(split[1]);
                            }
                            catch (Exception e)
                            {
                                logger.LogError($"Error parsing \"{configData.textureName}\"! Error is: \n{e.Message}");
                            }
                        }
                        HandleCustomTextureNames(material, texture, extractedValue, customTextureNames[split[0]]);
                    }
                    else if(customTextureNames.ContainsKey(configData.textureName))
                    {
                        HandleCustomTextureNames(material, texture, 0.6f, customTextureNames[configData.textureName]);
                    }
                    else
                    {
                        material.SetTexture(Shader.PropertyToID(configData.textureName), texture);
                    }

                    EnsureLinkedConfigs(configData);
                }
            }
        }

        private void HandleCustomTextureNames(Material material, Texture2D texture, float extractedValue, TextureType type)
        {
            if(Main.WriteLogs.Value)
            {
                Main.logger.LogInfo($"Handling custom texture name for type {type.ToString()}");
            }

            switch (type)
            {
                case TextureType.Emission:
                    if(extractedValue != -1)
                    {
                        material.SetFloat("_GlowStrength", 0);
                        material.SetFloat("_GlowStrengthNight", 0);
                        material.SetFloat("_EmissionLM", extractedValue);
                        material.SetFloat("_EmissionLMNight", extractedValue);
                    }                    
                    material.SetTexture("EmissionMap", texture);
                    break;
                case TextureType.LightColor:
                    Light[] lightArray = gameObject.GetComponentsInChildren<Light>();
                    Color32 averageColor = AverageColorFromTexture(texture);

                    if (lightArray != null)
                    {
                        foreach (Light light in lightArray)
                        {
                            light.color = averageColor;
                        }
                    }

                    if(material != null)
                    {
                        material.SetColor("_EmissionColor", averageColor);
                    }
                    break;
            }
        }

        private Color32 AverageColorFromTexture(Texture2D tex)
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

            Color32 color = new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);
            if (Main.WriteLogs.Value)
            {
                Main.logger.LogInfo($"Average color for texture \"{tex.name}\" is {color}");
            }
            return color;
        }


        private void EnsureLinkedConfigs(TexturePatchConfigData configData)
        {
            foreach (TexturePatchConfigData item in configDatas[Utilities.GetNameWithCloneRemoved(transform.name)])
            {
                if (!item.isVariation || item.variationChance == 1)
                {
                    continue;
                }

                if (item.linkedConfigNames.Count == 0)
                {
                    continue;
                }

                if (item.linkedConfigNames.Contains(configData.configName))
                {
                    item.variationAccepted = true;
                }
            }
        }
    }
}
