using Nautilus.Utility;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

using Random = UnityEngine.Random;
using TextureEditType = TextureReplacer.CustomTextureReplacer.TextureEditType;
using ConfigInfo = TextureReplacer.CustomTextureReplacer.ConfigInfo;
using TextureEdit = TextureReplacer.CustomTextureReplacer.ConfigInfo.TextureEdit;

namespace TextureReplacer
{
    internal class TextureReplacerHelper : MonoBehaviour
    {
        public ConfigInfo configInfo { get; private set; } = new();

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            SetUpTextures();
        }

        public void AddTextureData(ConfigInfo configInfo)
        {
            Main.logger.LogInfo($"Adding texture data for {configInfo.configName} | Texture edits = {configInfo.textureEdits}");

            for (int i = 0; i < configInfo.textureEdits.Count; i++)
            {
                TextureEdit edit = configInfo.textureEdits[i];
                Main.logger.LogInfo($"Trying to cache texture for {edit}");
                if (edit.editType == TextureEditType.Texture || edit.editType == TextureEditType.Sprite)
                {
                    edit.cachedTexture = ImageUtils.LoadTextureFromFile(Main.AssetFolderPath + $"/{edit.data}");
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
                    case TextureEditType.Keyword:
                        if(textureEdit.data.ToUpper() == "ENABLE")
                        {
                            material.EnableKeyword(textureEdit.propertyName);
                        }
                        else if(textureEdit.data.ToUpper() == "DISABLE")
                        {
                            material.DisableKeyword(textureEdit.propertyName);
                        }
                        break;
                }
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
            for (int i = 0; i < CustomTextureReplacer.textureConfigs.Count; i++)
            {
                ConfigInfo info = CustomTextureReplacer.textureConfigs[i];

                if (!info.isVariation || info.variationChance == 1)
                {
                    continue;
                }

                if (info.linkedConfigNames.Contains(configData.configName))
                {
                    info.variationAccepted = true;
                    continue;
                }

                if (info.prefabClassID == configData.prefabClassID && info.linkedConfigNames.Count == 0 && configData.linkedConfigNames.Count == 0)
                {
                    info.variationAccepted = true;
                }
            }
        }
    }
}
