using Nautilus.Utility;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

#region Class-Type Assignments
using Random = UnityEngine.Random;
using TextureEditType = TextureReplacer.CustomTextureReplacer.TextureEditType;
using ConfigInfo = TextureReplacer.CustomTextureReplacer.ConfigInfo;
using TextureEdit = TextureReplacer.CustomTextureReplacer.ConfigInfo.TextureEdit;
using System.Collections.Generic;
using System.Linq;
using static TextureReplacer.CustomTextureReplacer;
#endregion

namespace TextureReplacer
{
    internal class TextureReplacerHelper : MonoBehaviour
    {
        [SerializeField] public static Dictionary<string, List<ConfigInfo>> configInfos { get; private set; } = new();
        [SerializeField] public static Dictionary<string, bool> configIsPrefab { get; private set; } = new();
        private List<ConfigInfo> nonPrefabConfigInfos = new();

        private PrefabIdentifier identifier;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            SetUpTextures();
        }

        public void AddTextureData(ConfigInfo configInfo, bool isPrefab = true)
        {
            for (int i = 0; i < configInfo.textureEdits.Count; i++)
            {
                TextureEdit edit = configInfo.textureEdits[i];
                if (edit.editType == TextureEditType.Texture || edit.editType == TextureEditType.Sprite)
                {
                    edit.cachedTexture = ImageUtils.LoadTextureFromFile(Main.AssetFolderPath + $"/{edit.data}");
                }
            }

            PrefabIdentifier prefabIdentifier = GetComponent<PrefabIdentifier>();
            if(!configIsPrefab.ContainsKey(prefabIdentifier.ClassId))
            {
                configIsPrefab.Add(prefabIdentifier.ClassId, isPrefab);
            }
            else
            {
                configIsPrefab[prefabIdentifier.ClassId] = isPrefab;
            }

            if (isPrefab)
            {
                if (!configInfos.ContainsKey(prefabIdentifier.ClassId))
                {
                    configInfos.Add(prefabIdentifier.ClassId, new List<ConfigInfo>());
                }

                var list = configInfos[prefabIdentifier.ClassId];
                list.Add(configInfo);
                configInfos[prefabIdentifier.ClassId] = list;
            }
            else
            {
                nonPrefabConfigInfos.Add(configInfo);
            }
        }

        private void SetUpTextures()
        {
            identifier = GetComponent<PrefabIdentifier>();
            List<ConfigInfo> infos = configIsPrefab[identifier.ClassId] ? configInfos[identifier.ClassId] : nonPrefabConfigInfos;

            foreach (ConfigInfo configInfo in infos)
            {
                SwapTextures(configInfo);
            }
        }

        private void SwapTextures(ConfigInfo configInfo, bool checkForLinkedConfigs = true)
        {
            if(Main.WriteLogs.Value)
            {
                Main.logger.LogInfo($"Loading config ({configInfo.configName}) | Hierarchy path = {configInfo?.rendererHierarchyPath}");
            }

            if (configInfo.isVariation && Random.Range(0f, 1f) < configInfo.variationChance)
            {
                return;
            }

            if(checkForLinkedConfigs)
            {
                var linkedConfigs = GetLinkedConfigs(configInfo);
                foreach (var config in linkedConfigs)
                {
                    SwapTextures(config, false);
                }
            }

            Renderer targetRenderer = transform.Find(configInfo.rendererHierarchyPath)?.GetComponent<Renderer>();
            if (targetRenderer == null && !configInfo.textureEdits.Any(i => i.editType == TextureEditType.Light))
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

                Material material = targetRenderer?.materials[textureEdit.materialIndex];

                if (textureEdit.propertyName == "Sprite")
                {
                    HandleSpriteSwapping(textureEdit.cachedTexture, targetRenderer.gameObject);
                    continue;
                }

                switch (textureEdit.editType)
                {
                    case TextureEditType.Texture:
                        material.SetTexture(textureEdit.propertyName, textureEdit.cachedTexture);

                        break;
                    case TextureEditType.Color:
                        material.SetColor(textureEdit.propertyName, ParseV4FromString(textureEdit.data, ','));
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
                        material.SetVector(textureEdit.propertyName, ParseV4FromString(textureEdit.data, ',', 0));
                        break;
                    case TextureEditType.Keyword:
                        if (textureEdit.data.ToUpper() == "ENABLE")
                        {
                            material.EnableKeyword(textureEdit.propertyName);
                        }
                        else if (textureEdit.data.ToUpper() == "DISABLE")
                        {
                            material.DisableKeyword(textureEdit.propertyName);
                        }
                        break;
                    case TextureEditType.Light:
                        Light[] lights = transform.Find(configInfo.rendererHierarchyPath).GetComponentsInChildren<Light>();

                        lights.ForEach(i => i.color = ParseV4FromString(textureEdit.data, ','));
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

        private List<ConfigInfo> GetLinkedConfigs(ConfigInfo configData)
        {
            var linkedConfigs = new List<ConfigInfo>();

            for (int i = 0; i < textureConfigs.Count; i++)
            {
                ConfigInfo info = textureConfigs[i];

                if (configData.prefabClassID == info.prefabClassID && configData.linkedConfigNames.Count == 0 && info.linkedConfigNames.Count == 0)
                {
                    linkedConfigs.Add(info);
                    continue;
                }

                if(configData.linkedConfigNames.Contains(info.configName) || info.linkedConfigNames.Contains(configData.configName))
                {
                    linkedConfigs.Add(info);
                }
            }

            return linkedConfigs;
        }

        private Vector4 ParseV4FromString(string v4String, char separatorChar, int wFallbackValue = 1)
        {
            try
            {
                string[] vectorValues = v4String.Split(separatorChar);

                float x, y, z;
                float w = wFallbackValue;
                x = float.Parse(vectorValues[0]);
                y = float.Parse(vectorValues[1]);
                z = float.Parse(vectorValues[2]);

                if (vectorValues.Length > 3)
                {
                    w = float.Parse(vectorValues[3]);
                }

                return new Vector4(x, y, z, w);
            }
            catch (Exception e)
            {
                Main.logger.LogError($"Error parsing vector values for {v4String}! \n{e.Message}");
                return Vector4.zero;
            }
        }
    }
}
