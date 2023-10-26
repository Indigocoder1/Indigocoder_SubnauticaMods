using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace SuitLib
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    internal class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.SuitLib";
        private const string pluginName = "SuitLib";
        private const string versionString = "1.0.4";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);
        internal static Eatable waterPefab;

        private IEnumerator Start()
        {
            logger = Logger;

            harmony.PatchAll();

            CoroutineTask<GameObject> stillsuitTask = CraftData.GetPrefabForTechTypeAsync(TechType.WaterFiltrationSuit);
            yield return stillsuitTask;
            waterPefab = stillsuitTask.result.value.GetComponent<Stillsuit>().waterPrefab;

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
