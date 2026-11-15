using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SealGrapplingTurret.Items;
using SealSubMod;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SealGrapplingTurret
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("SealSub", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "com.Indigocoder.SealGrapplingTurret";
        private const string pluginName = "Seal Grappling Turret";
        private const string versionString = "1.0.0";
        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private static readonly Assembly executingAssembly = Assembly.GetExecutingAssembly();
        public static string recipeFilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "Recipes"), "ModuleRecipe.json");

        public static EquipmentType sealModuleEquipmentType;
        public static GameObject hookPrefab;

        private IEnumerator Start()
        {
            logger = Logger;

            EnumHandler.TryGetValue("SealModule", out sealModuleEquipmentType);
            GrappleModule.Patch();

            CoroutineTask<GameObject> exosuitTask = CraftData.GetPrefabForTechTypeAsync(TechType.Exosuit);
            yield return exosuitTask;

            Exosuit exosuit = exosuitTask.result.value.GetComponent<Exosuit>();
            Transform armTransform = exosuit.GetArmPrefab(TechType.ExosuitGrapplingArmModule).transform;
            hookPrefab =  armTransform.GetComponent<ExosuitGrapplingArm>().hookPrefab;

            harmony.PatchAll();
            Plugin.RegisterUpgradeModuleFunctionalities(executingAssembly);

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
