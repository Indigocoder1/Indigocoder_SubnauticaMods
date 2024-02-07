using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Crafting;
using Nautilus.Json.Converters;
using Nautilus.Utility;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using VariableGravityCannon.Items;

namespace VariableGravityCannon
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.VariableGravityCannon";
        private const string pluginName = "Variable Gravity Cannon";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        public static ConfigEntry<float> repulsionModeFireEnergy;
        public static ConfigEntry<float> propulsionModeFireEnergy;
        public static ConfigEntry<float> propulsionModePerSecondEnergy;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static string RecipesFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Recipes");

        public static Texture2D variablePropTex;
        public static Texture2D variableRepulsionTex;

        public static Texture2D variablePropIllum;
        public static Texture2D variableRepulsionIllum;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private IEnumerator Start()
        {
            logger = Logger;
            SetupConfigs();

            CoroutineTask<GameObject> repulsionTask = CraftData.GetPrefabForTechTypeAsync(TechType.RepulsionCannon);
            yield return repulsionTask;

            LoadTextures();
            VariableGravityCannon_Craftable.Patch(repulsionTask.result.value.GetComponent<RepulsionCannon>());
            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void LoadTextures()
        {
            variablePropTex = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolderPath, "variablePropTex.png"));
            variableRepulsionTex = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolderPath, "variableRepulsionTex.png"));

            variablePropIllum = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolderPath, "variablePropulsionIllum.png"));
            variableRepulsionIllum = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolderPath, "variableRepulsionIllum.png"));
        }

        private void SetupConfigs()
        {
            repulsionModeFireEnergy = Config.Bind("VariableGravityCannon", "Repulsion mode fire energy draw", 4f);
            propulsionModeFireEnergy = Config.Bind("VariableGravityCannon", "Propulsion mode fire energy draw", 4f);
            propulsionModePerSecondEnergy = Config.Bind("VariableGravityCannon", "Repulsion mode constant energy draw", 0.7f);
        }

        public static RecipeData GetRecipeFromJson(string path)
        {
            var content = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<RecipeData>(content, new CustomEnumConverter());
        }
    }
}
