using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SuitLib.API.JSON;

internal static class JsonLoader<T>
{
    public static List<T> LoadJsons(string folderPath)
    {
        List<T> configData = new();
        foreach (var file in Directory.EnumerateFiles(folderPath, "*.json"))
        {
            List<T> tempData = new ();
            try
            {
                tempData = LoadJson(file);
            }
            catch (Exception e)
            {
                Main.logger.LogError($"Error loading JSON at {file} \nMessage is: {e.Message}");
            }

            configData.AddRange(tempData);
        }

        return configData;
    }

    private static List<T> LoadJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        string data = File.ReadAllText(filePath);
        try
        {
            List<T> List = JsonConvert.DeserializeObject<List<T>>(data);
            return List;
        }
        catch
        {
            return null;
        }
    }

    public static void SaveToJson(List<T> saveData, string filePath)
    {
        var textureConfigJson = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(filePath, textureConfigJson);
    }
}