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
            if(configDatas == null)
            {
                configDatas = new Dictionary<string, List<TexturePatchConfigData>>();
            }

            if(filePaths == null)
            {
                filePaths = new Dictionary<string, List<string>>();
            }

            if(textures == null)
            {
                textures = new Dictionary<string, List<Texture2D>>();
            }

            if (!configDatas.ContainsKey(transform.name))
            {
                configDatas.Add(transform.name, new List<TexturePatchConfigData>());
            }
            configDatas[transform.name].Add(configData);

            if (!filePaths.ContainsKey(transform.name))
            {
                filePaths.Add(transform.name, new List<string>());
            }
            filePaths[transform.name].Add(AssetFolderPath + $"/{configData.fileName}");

            if(!textures.ContainsKey(transform.name))
            {
                textures.Add(transform.name, new List<Texture2D>());
            }

            textures[transform.name].Add(ImageUtils.LoadTextureFromFile(AssetFolderPath + $"/{configData.fileName}"));
        }

        private void SetUpTextures()
        {
            string nameWithoutClone = Utilities.GetNameWithCloneRemoved(transform.name);

            for (int i = 0; i < configDatas[nameWithoutClone].Count; i++)
            {
                TexturePatchConfigData configData = configDatas[nameWithoutClone][i];
                if (configData.variationAccepted || !configData.isVariation || Random.Range(0f, 1f) <= configData.variationChance)
                {
                    Texture2D texture = textures[nameWithoutClone][i];

                    Renderer targetRenderer = transform.Find(configData.rendererHierarchyPath).GetComponent<Renderer>();
                    Material material = targetRenderer.materials[configData.materialIndex];

                    material.SetTexture(Shader.PropertyToID(configData.textureName), texture);

                    EnsureLinkedConfigs(configData);
                }
            }
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
