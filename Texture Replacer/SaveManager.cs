using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace TextureReplacer
{
    internal static class SaveManager
    {
        private static string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config/TextureConfig.json");

        public static void SaveToJson(Main.LifepodTextureConfigList saveData)
        { 
            var textureConfigJson = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(filePath, textureConfigJson);
            Console.WriteLine($"Data saved to JSON at {filePath}");
        }

        public static Main.LifepodTextureConfigList LoadFromJson()
        {
            string data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Main.LifepodTextureConfigList>(data);
        }
    }
}
