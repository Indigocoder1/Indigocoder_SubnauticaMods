using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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

        public static List<Main.TexturePatchConfigData> LoadConfigs(string folderPath)
        {
            List<Main.TexturePatchConfigData> configDatas = new List<Main.TexturePatchConfigData>();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.json"))
            {
                List<Main.TexturePatchConfigData> tempData = new List<Main.TexturePatchConfigData>();
                try
                {
                    tempData = LoadFromJson(file);
                }
                catch (Exception e)
                {
                    Main.logger.LogError($"Error loading JSON at {file} \nMessage is {e.Message}");
                }

                configDatas.AddRange(tempData);
            }

            return configDatas;
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
