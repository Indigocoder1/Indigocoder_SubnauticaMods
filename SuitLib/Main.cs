using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SuitLib.API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SuitLib
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    internal class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.SuitLib";
        private const string pluginName = "SuitLib";
        private const string versionString = "1.0.6";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);
        private static string jsonFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Json Files");
        internal static string jsonTexturesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Json Textures");
        internal static Eatable waterPefab;

        private IEnumerator Start()
        {
            logger = Logger;
            
            harmony.PatchAll();

            CoroutineTask<GameObject> stillsuitTask = CraftData.GetPrefabForTechTypeAsync(TechType.WaterFiltrationSuit);
            yield return stillsuitTask;
            waterPefab = stillsuitTask.result.value.GetComponent<Stillsuit>().waterPrefab;

            SaveExampleData();
            LoadJsons();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void SaveExampleData()
        {
            if (!File.Exists(Path.Combine(jsonFolder, "exampleSuit.json")))
            {
                logger.LogWarning("Saving example data. You may see Nautilus file errors");
                Dictionary<string, string> suitKVP = new Dictionary<string, string> { { "_MainTex", "MySuitTexture.png" } };
                Dictionary<string, string> armsKVP = new Dictionary<string, string> { { "_MainTex", "MyArmsTexture.png" } };
                ModdedSuit exampleSuit = new ModdedSuit(suitKVP, armsKVP, ModdedSuitsManager.VanillaModel.Reinforced,
                    ModdedSuitsManager.Modifications.Reinforced);

                JsonLoader<ModdedSuit>.SaveToJson(new List<ModdedSuit> { exampleSuit }, Path.Combine(jsonFolder, "exampleSuit.json"));
            }

            if (!File.Exists(Path.Combine(jsonFolder, "exampleGloves.json")))
            {
                logger.LogWarning("Saving example data. You may see Nautilus file errors");
                Dictionary<string, string> glovesKVP = new Dictionary<string, string> { { "_MainTex", "MyGlovesTexture.png" } };
                ModdedGloves exampleGloves = new ModdedGloves(glovesKVP, ModdedSuitsManager.VanillaModel.Reinforced, ModdedSuitsManager.Modifications.Reinforced);

                JsonLoader<ModdedGloves>.SaveToJson(new List<ModdedGloves> { exampleGloves }, Path.Combine(jsonFolder, "exampleGloves.json"));
            }
        }

        private void LoadJsons()
        {
            foreach (ModdedSuit suit in JsonLoader<ModdedSuit>.LoadJsons(jsonFolder))
            {
                if (suit != null)
                {
                    ModdedSuitsManager.AddModdedSuit(suit);
                }
            }

            foreach (ModdedGloves gloves in JsonLoader<ModdedGloves>.LoadJsons(jsonFolder))
            {
                if (gloves != null)
                {
                    ModdedSuitsManager.AddModdedGloves(gloves);
                }
            }
        }
    }
}
