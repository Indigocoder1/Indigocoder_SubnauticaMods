using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TextureReplacer
{
    internal class Texture_Replacer : MonoBehaviour
    {
        public void ReplaceTexture(Material material, string textureFileName)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + $"/{textureFileName}";
            Texture2D texture = ImageUtils.LoadTextureFromFile(filePath);

            Main.logger.LogInfo("Material successfully replaced!");
            material.SetTexture("_MainTex", texture);
        }
    }
}
