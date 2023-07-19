using Nautilus.Utility;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TextureReplacer
{
    internal class TextureReplacerHelper : MonoBehaviour
    {
        public void ReplaceTexture(Material material, string textureFileName)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + $"/{textureFileName}";
            Texture2D texture = null;
            try
            {
                texture = ImageUtils.LoadTextureFromFile(filePath);
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error reading file {textureFileName}! Exception: {e.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            material.SetTexture("_MainTex", texture);
            Main.logger.LogInfo("Texture successfully replaced!");
        }
    }
}
