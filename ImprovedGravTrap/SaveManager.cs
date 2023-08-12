using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
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
            return JsonConvert.DeserializeObject<List<TechTypeList>>(data);
        }
    }
}
