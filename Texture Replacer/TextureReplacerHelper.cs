using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;
using IndigocoderLib;
using System.Collections;
using static TextureReplacer.Main;
using Random = UnityEngine.Random;
using System;
using static TextureReplacer.CustomTextureReplacer;
using UnityEngine.UI;
using static TextureReplacer.CustomTextureReplacer.ConfigInfo;
using Valve.VR;

namespace TextureReplacer
{
    internal class TextureReplacerHelper : MonoBehaviour
    {
        public ConfigInfo configInfo { get; private set; } = new();

        public Texture2D cachedTexture { get; private set; }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            SetUpTextures();
        }

        public void AddTextureData(ConfigInfo configInfo)
        {
            for (int i = 0; i < configInfo.textureEdits.Count; i++)
            {
                TextureEdit edit = configInfo.textureEdits[i];
                if (edit.editType == TextureEditType.Texture || edit.editType == TextureEditType.Sprite)
                {
                    edit.cachedTexture = ImageUtils.LoadTextureFromFile(AssetFolderPath + $"/{edit.data}");
                }
            }

            this.configInfo = configInfo;
        }

        private void SetUpTextures()
        {
            if(Random.Range(0f, 1f) > configInfo.variationChance && configInfo.isVariation && !configInfo.variationAccepted)
            {
                return;
            }

            EnsureLinkedConfigs(configInfo);
            Renderer targetRenderer = transform.Find(configInfo.rendererHierarchyPath).GetComponent<Renderer>();
            if(targetRenderer == null)
            {
                Main.logger.LogError($"Target renderer not found at path {configInfo.rendererHierarchyPath} from {transform}! Aborting texture edit load for {configInfo.configName}");
                return;
            }

            foreach (var textureEdit in configInfo.textureEdits)
            {
                if (Main.WriteLogs.Value)
                {
                    Main.logger.LogInfo($"Loading texture edits on {transform.name} for {textureEdit.propertyName}");
                }

                Material material = targetRenderer.materials[textureEdit.materialIndex];

                if (textureEdit.propertyName == "Sprite")
                {
                    HandleSpriteSwapping(textureEdit.cachedTexture, targetRenderer.gameObject);
                    continue;
                }

                switch(textureEdit.editType)
                {
                    case TextureEditType.Texture:
                        material.SetTexture(textureEdit.propertyName, textureEdit.cachedTexture);
                        break;
                    case TextureEditType.Color:
                        string[] colorValues = textureEdit.data.Split(',');
                        float r, g, b;
                        float a = 1;
                        try
                        {
                            r = float.Parse(colorValues[0]);
                            g = float.Parse(colorValues[1]);
                            b = float.Parse(colorValues[2]);
                            if (colorValues.Length > 3)
                            {
                                a = float.Parse(colorValues[3]);
                            }
                        }
                        catch(Exception e)
                        {
                            Main.logger.LogError($"Error parsing color values on {configInfo.configName}. Aborting color set!\n{e.Message}");
                            return;
                        }

                        material.SetColor(textureEdit.propertyName, new Color(r, g, b, a));
                        break;
                    case TextureEditType.Float:
                        float value = 0;
                        try
                        {
                            value = float.Parse(textureEdit.data);
                        }
                        catch (Exception e)
                        {
                            Main.logger.LogError($"Error parsing float value on {configInfo.configName}. Aborting float set!\n{e.Message}");
                        }

                        material.SetFloat(textureEdit.propertyName, value);
                        break;
                    case TextureEditType.Vector:
                        string[] vectorValues = textureEdit.data.Split(',');
                        float x, y, z;
                        float w = 1;
                        try
                        {
                            x = float.Parse(vectorValues[0]);
                            y = float.Parse(vectorValues[1]);
                            z = float.Parse(vectorValues[2]);
                            
                            if (vectorValues.Length > 3)
                            {
                                w = float.Parse(vectorValues[3]);
                            }
                        }
                        catch (Exception e)
                        {
                            Main.logger.LogError($"Error parsing color values on {configInfo.configName}. Aborting color set!\n{e.Message}");
                            return;
                        }

                        material.SetVector(textureEdit.propertyName, new Vector4(x, y, z, w));
                        break;
                }
            }
        }

        private void HandleCustomTextureNames(Material material, Texture2D texture, float extractedValue, string key)
        {
            TextureType type = TextureType.Value;
            if (customTextureNames.ContainsKey(key))
            {
                type = Main.customTextureNames[key];
            }

            if (Main.WriteLogs.Value)
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
                    material.SetTexture("_EmissionMap", texture);
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
                case TextureType.Color:
                    material.SetColor("_Color", AverageColorFromTexture(texture));
                    break;
                case TextureType.Value:
                    material.SetFloat(key, extractedValue);
                    break;
            }
        }

        private void HandleSpriteSwapping(Texture2D tex, GameObject go)
        {
            if(!go.TryGetComponent<Image>(out var image))
            {
                Main.logger.LogError($"Image component not found on {go}! A sprite on it was trying to be swapped.");
                return;
            }

            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);
        }


        private void EnsureLinkedConfigs(ConfigInfo configData)
        {
            foreach (TexturePatchConfigData item in configDatas[Utilities.GetNameWithCloneRemoved(transform.name)])
            {
                if (!item.isVariation || item.variationChance == 1)
                {
                    continue;
                }

                if (item.linkedConfigNames.Contains(configData.configName))
                {
                    item.variationAccepted = true;
                }
                
                if (item.prefabClassID == configData.prefabClassID && item.linkedConfigNames.Count == 0 && configData.linkedConfigNames.Count == 0)
                {
                    item.variationAccepted = true;
                }
            }
        }
    }
}
