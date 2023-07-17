using System;
using System.IO;
using Newtonsoft.Json;
using BepInEx;
using System.Collections.Generic;

namespace TextureReplacer
{
    internal static class SaveManager
    {
        private static string filePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureConfig.json");

        public static void SaveToJson(List<LifepodTextureReplacer.LifepodConfigData> saveData)
        { 
            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(filePath, textureConfigJson);
            Console.WriteLine($"Data saved to JSON at {filePath}");
        }

        public static List<LifepodTextureReplacer.LifepodConfigData> LoadFromJson()
        {
            string data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<LifepodTextureReplacer.LifepodConfigData>>(data);
        }
    }
}
