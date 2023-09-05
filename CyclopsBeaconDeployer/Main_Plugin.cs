﻿using BepInEx;
using BepInEx.Logging;
using CyclopsBeaconDeployer.Items;
using HarmonyLib;
using IndigocoderLib;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CyclopsBeaconDeployer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.CyclopsBeaconDeployer";
        private const string pluginName = "Cyclops Beacon Deployer";
        private const string versionString = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);
        public static ManualLogSource logger;

        public static string AssetFolderPath { get; private set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle assetBundle { get; private set; }

        public static GameObject nameInputFieldGO { get; private set; }
        public static GameObject decoyPrefab { get; private set; }
        public static GameObject beaconPrefab { get; private set; }

        private IEnumerator Start()
        {
            logger = Logger;

            if (PiracyDetector.TryFindPiracy())
            {
                yield break;
            }

            CoroutineTask<GameObject> decoyTask = CraftData.GetPrefabForTechTypeAsync(TechType.CyclopsDecoy);
            yield return decoyTask;

            CoroutineTask<GameObject> beaconTask = CraftData.GetPrefabForTechTypeAsync(TechType.Beacon);
            yield return beaconTask;

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetFolderPath, "beacondeployer"));
            nameInputFieldGO = assetBundle.LoadAsset<GameObject>("BeaconNameInput");

            decoyTask.result.value.AddComponent<PlaceTool>();
            decoyPrefab = decoyTask.result.value;

            beaconPrefab = beaconTask.result.value;

            CraftData.equipmentTypes[TechType.Beacon] = EquipmentType.DecoySlot;

            BeaconDeployModule.Patch();
            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
