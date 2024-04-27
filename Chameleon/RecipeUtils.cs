using Nautilus.Crafting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System;
using Nautilus.Json.Converters;

namespace Chameleon
{
    public static class RecipeUtils
    {
        private static Dictionary<TechType, RecipeData> cachedRecipes = new();
        public static RecipeData TryGetRecipeFromJson(TechType recipeToGet, string filePath = null)
        {
            if (cachedRecipes.TryGetValue(recipeToGet, out var recipe)) return recipe;

            if (string.IsNullOrEmpty(filePath)) filePath = GetChameleonRecipeFile(recipeToGet);
            var content = File.ReadAllText(filePath);

            RecipeData data = null;
            try
            {
                data = JsonConvert.DeserializeObject<RecipeData>(content, new CustomEnumConverter());
            }
            catch (Exception e)
            {
                Main_Plugin.logger.LogError($"Error deserializing recipe at {filePath}: {e.Message}");
            }

            if (data == null) return null;

            cachedRecipes.Add(recipeToGet, data);

            return data;
        }

        private static string GetChameleonRecipeFile(TechType techType)
        {
            return Path.Combine(Main_Plugin.RecipesFolderPath, $"{techType}.json");
        }
    }
}
