using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Nautilus.Json.Converters;

namespace ImprovedGravTrap
{
    internal static class SaveManager
    {
        public static void SaveToJson(List<TechTypeList> saveData, string filePath, string folderFilePath = null)
        {
            if(!string.IsNullOrEmpty(folderFilePath))
            {
                if (!Directory.Exists(folderFilePath))
                {
                    Directory.CreateDirectory(folderFilePath);
                }
            }

            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented, new CustomEnumConverter());
            File.WriteAllText(filePath, textureConfigJson);
            Console.WriteLine($"Data saved to JSON at {filePath}");
        }

        public static List<TechTypeList> LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<TechTypeList>>(data, new CustomEnumConverter());
        }
    }
}
