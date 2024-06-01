using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using static TextureReplacer.CustomTextureReplacer;
using Nautilus.Json.Converters;

namespace TextureReplacer.Saving
{
    internal static class SaveManager<T>
    {
        public static void SaveToJson(List<T> saveData, string filePath, string folderFilePath)
        {
            if (!Directory.Exists(folderFilePath))
            {
                Directory.CreateDirectory(folderFilePath);
            }

            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented, new CustomEnumConverter());
            File.WriteAllText(filePath, textureConfigJson);
            Main.logger.LogInfo($"Data saved to JSON at {filePath}");
        }

        public static List<T> LoadJsons(string folderPath)
        {
            List<T> configDatas = new List<T>();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.json"))
            {
                List<T> tempData = null;
                try
                {
                    tempData = LoadJson(file);
                }
                catch (Exception e)
                {
                    Main.logger.LogError($"Error loading JSON at {file} \nMessage is: {e.Message}");
                }

                if (tempData == null) continue;

                configDatas.AddRange(tempData);
            }

            return configDatas;
        }

        public static List<T> LoadJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string data = File.ReadAllText(filePath);
            JArray request = JsonConvert.DeserializeObject<JArray>(data, new CustomEnumConverter());

            //A bit horrendous but should fix the return of the incorrect type
            if (typeof(T) == typeof(TexturePatchConfigData) && !request.Children<JObject>().Properties().Any(i => i.Name == "fileName"))
            {
                return null;
            }

            if (typeof(T) == typeof(ConfigInfo) && !request.Children<JObject>().Properties().Any(i => i.Name == "textureEdits"))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<List<T>>(data, new CustomEnumConverter());
        }

        public static List<LifepodTextureReplacer.LifepodConfigData> LoadLifepodConfigs(string folderPath)
        {
            List<LifepodTextureReplacer.LifepodConfigData> configDatas = new List<LifepodTextureReplacer.LifepodConfigData>();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.json"))
            {
                List<LifepodTextureReplacer.LifepodConfigData> tempData = new List<LifepodTextureReplacer.LifepodConfigData>();
                try
                {
                    tempData = LoadLifepodConfigFromJson(file);
                }
                catch (Exception e)
                {
                    Main.logger.LogError($"Error loading JSON at {file} \nMessage is: {e.Message}");
                }

                configDatas.AddRange(tempData);
            }

            return configDatas;
        }

        public static void SaveLifepodConfigToJson(List<LifepodTextureReplacer.LifepodConfigData> saveData, string filePath, string folderFilePath)
        {
            if (!Directory.Exists(folderFilePath))
            {
                Directory.CreateDirectory(folderFilePath);
            }

            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(filePath, textureConfigJson);
            Main.logger.LogInfo($"Data saved to JSON at {filePath}");
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
