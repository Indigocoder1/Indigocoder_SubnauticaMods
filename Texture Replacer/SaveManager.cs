using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextureReplacer
{
    internal static class SaveManager
    {
        public static void SaveToJson(List<Main.TexturePatchConfigData> saveData, string filePath, string folderFilePath)
        {
            if (!Directory.Exists(folderFilePath))
            {
                Directory.CreateDirectory(folderFilePath);
            }

            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(filePath, textureConfigJson);
            Console.WriteLine($"Data saved to JSON at {filePath}");
        }

        public static List<Main.TexturePatchConfigData> LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Main.TexturePatchConfigData>>(data);
        }

        public static void SaveLifepodConfigToJson(List<LifepodTextureReplacer.LifepodConfigData> saveData, string filePath, string folderFilePath)
        {
            if (!Directory.Exists(folderFilePath))
            {
                Directory.CreateDirectory(folderFilePath);
            }

            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(filePath, textureConfigJson);
            Console.WriteLine($"Data saved to JSON at {filePath}");
        }

        public static List<LifepodTextureReplacer.LifepodConfigData> LoadLifepodConfigFromJson(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return null;
            }

            string data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<LifepodTextureReplacer.LifepodConfigData>>(data);
        }
    }
}
