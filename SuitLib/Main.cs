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

            LoadJsons();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
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
