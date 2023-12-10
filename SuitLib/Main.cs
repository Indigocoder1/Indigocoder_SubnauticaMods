using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;
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
        private const string versionString = "1.0.8";

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

            LoadJsons();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void LoadJsons()
        {
            List<JsonSuit> suits = JsonLoader<JsonSuit>.LoadJsons(jsonFolder);

            foreach (JsonSuit suit in suits)
            {
                if(suit == null) continue;

                Dictionary<string, Texture2D> suitKVPs = new Dictionary<string, Texture2D>();
                Dictionary<string, Texture2D> armKVPs = new Dictionary<string, Texture2D>();

                if(suit.suitFileNamePairs == null || suit.armFileNamePairs == null)
                {
                    continue;
                }

                foreach (var suitKey in suit.suitFileNamePairs.Keys)
                {
                    string path = Path.Combine(jsonTexturesFolder, suit.suitFileNamePairs[suitKey]);
                    suitKVPs.Add(suitKey, ImageUtils.LoadTextureFromFile(path));
                }
                foreach (var armsKey in suit.armFileNamePairs.Keys)
                {
                    string path = Path.Combine(jsonTexturesFolder, suit.armFileNamePairs[armsKey]);
                    armKVPs.Add(armsKey, ImageUtils.LoadTextureFromFile(path));
                }

                ModdedSuit moddedSuit = new ModdedSuit(suitKVPs, armKVPs, suit.vanillaModel, ModdedSuitsManager.suitModelTechTypes[suit.vanillaModel], suit.modifications, suit.stillsuitValues);
                moddedSuit.jsonSuit = true;
                ModdedSuitsManager.AddModdedSuit(moddedSuit);
            }

            logger.LogInfo("Finished loading suit Jsons");

            List<JsonGloves> gloves = JsonLoader<JsonGloves>.LoadJsons(jsonFolder);

            foreach (JsonGloves item in gloves)
            {
                if (item == null) continue;

                if (item.gloveTexturePairs == null)
                {
                    continue;
                }

                Dictionary<string, Texture2D> armKVPs = new Dictionary<string, Texture2D>();
                foreach (var glovesKey in item.gloveTexturePairs.Keys)
                {
                    string path = Path.Combine(jsonTexturesFolder, item.gloveTexturePairs[glovesKey]);
                    armKVPs.Add(glovesKey, ImageUtils.LoadTextureFromFile(path));
                }

                ModdedGloves moddedGloves = new ModdedGloves(armKVPs, item.vanillaModel, ModdedSuitsManager.gloveModelTechTypes[item.vanillaModel], item.modifications, item.stillsuitValues);
                moddedGloves.jsonGloves = true;
                ModdedSuitsManager.AddModdedGloves(moddedGloves);
            }
        }
    }
}
