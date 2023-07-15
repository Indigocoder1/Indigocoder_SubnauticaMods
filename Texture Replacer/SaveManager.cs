using System;
using System.IO;
using System.Reflection;
using static TextureReplacer.Main;
using Newtonsoft.Json;

namespace TextureReplacer
{
    internal static class SaveManager
    {
        private static string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config/TextureConfig.json");

        public static void SaveToJson(TextureConfigList saveData)
        { 
            var textureConfigJson = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(filePath, textureConfigJson);
            Console.WriteLine($"Data saved to JSON at {filePath}");
        }

        public static TextureConfigList LoadFromJson()
        {
            string data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<TextureConfigList>(data);
        }
    }
}
