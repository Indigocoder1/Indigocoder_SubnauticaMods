using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using SuitLib.Patches;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace SuitLib
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.github.tinyhoot.DeathrunRemade", BepInDependency.DependencyFlags.SoftDependency)]
    internal class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.SuitLib";
        private const string pluginName = "SuitLib";
        private const string versionString = "1.0.3";

        public static ManualLogSource logger;
        private static readonly Harmony harmony = new Harmony(myGUID);
        internal static Eatable waterPefab;

        private IEnumerator Start()
        {
            logger = Logger;

            harmony.PatchAll();
            if(Chainloader.PluginInfos.ContainsKey("com.github.tinyhoot.DeathrunRemade"))
            {
                MethodBase method = AccessTools.Method(typeof(Deathrun_Crush_Patch), "GetCrushDepth_Patch", new Type[] { typeof(TechType), typeof(float) });
                HarmonyMethod postfix = new HarmonyMethod();
                harmony.Patch(method, postfix);
            }

            CoroutineTask<GameObject> stillsuitTask = CraftData.GetPrefabForTechTypeAsync(TechType.WaterFiltrationSuit);
            yield return stillsuitTask;
            waterPefab = stillsuitTask.result.value.GetComponent<Stillsuit>().waterPrefab;

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
