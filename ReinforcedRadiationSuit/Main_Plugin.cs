using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using System.IO;
using BepInEx.Configuration;
using IndigocoderLib;
using SuitLib;
using Nautilus.Utility;
using UnityEngine;
using System.Collections.Generic;
using Nautilus.Crafting;
using Nautilus.Json.Converters;
using Newtonsoft.Json;
using ReinforcedRadiationSuit.Items;
using BepInEx.Bootstrap;
using System;

namespace ReinforcedRadiationSuit
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Indigocoder.SuitLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.github.tinyhoot.DeathrunRemade", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.ReinforcedRadiationSuit";
        private const string pluginName = "Reinforced Radiation Suit";
        private const string versionString = "1.0.5";
        public static ManualLogSource logger;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static string RecipesFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Recipes");

        public static RecipeData SuitRecipe;

        internal static Texture2D suitMainTex;
        internal static Texture2D armsMainTex;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
        {
            logger = Logger;

            PiracyDetector.TryFindPiracy();

            harmony.PatchAll();

            RebreatherRadiationHelmet_Craftable.Patch();
            ReinforcedRadiationGloves_Craftable.Patch();
            InitializeRecipes();

            ReinforcedRadiationSuit_Craftable.Patch();

            InitializeSuits();

            if(Chainloader.PluginInfos.ContainsKey("com.github.tinyhoot.DeathrunRemade"))
            {
                gameObject.AddComponent<DeathrunComponentAdder>();
            }

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void InitializeRecipes()
        {
            SuitRecipe = GetRecipeFromJson(Path.Combine(RecipesFolderPath, "SuitRecipe.json"));
        }

        private void InitializeSuits()
        {
            string suitFilePath = Path.Combine(AssetsFolderPath, "Textures", "reinforcedRadiation_Body.png");
            suitMainTex = ImageUtils.LoadTextureFromFile(suitFilePath);

            string armsFilePath = Path.Combine(AssetsFolderPath, "Textures", "reinforcedRadiation_Arms.png");
            armsMainTex = ImageUtils.LoadTextureFromFile(armsFilePath);

            Dictionary<string, Texture2D> suitKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", suitMainTex }, { "_SpecTex", suitMainTex } };
            Dictionary<string, Texture2D> armKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", armsMainTex }, { "_SpecTex", armsMainTex } };

            ModdedSuit warpSuit = new ModdedSuit(suitKeyValuePairs, armKeyValuePairs, ModdedSuitsManager.VanillaModel.Radiation, ReinforcedRadiationSuit_Craftable.techType,
                ModdedSuitsManager.Modifications.Reinforced, tempValue: 0f);
            ModdedGloves warpGloves = new ModdedGloves(armKeyValuePairs, ModdedSuitsManager.VanillaModel.Radiation, ReinforcedRadiationGloves_Craftable.techType,
                ModdedSuitsManager.Modifications.Reinforced, tempValue: 0f);

            ModdedSuitsManager.AddModdedSuit(warpSuit);
            ModdedSuitsManager.AddModdedGloves(warpGloves);
        }

        private static RecipeData GetRecipeFromJson(string path)
        {
            var content = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<RecipeData>(content, new CustomEnumConverter());
        }
    }

    internal class DeathrunComponentAdder : MonoBehaviour
    {
        private void Start()
        {
            Type DeathrunAPI = AccessTools.TypeByName("DeathrunRemade.DeathrunAPI");

            MethodInfo AddSuitCrushDepth = DeathrunAPI.GetMethod("AddSuitCrushDepth", new Type[] { typeof(TechType), typeof(IEnumerable<float>) });
            MethodInfo AddNitrogenModifier = DeathrunAPI.GetMethod("AddNitrogenModifier", new Type[] { typeof(TechType), typeof(IEnumerable<float>) });

            AddSuitCrushDepth.Invoke(null, new object[] { ReinforcedRadiationSuit_Craftable.techType, new List<float> { 900, 700 } });
            AddNitrogenModifier.Invoke(null, new object[] { RebreatherRadiationHelmet_Craftable.techType, new List<float> { .3f, .15f, 0f } });

            Destroy(this);
        }
    }
}
