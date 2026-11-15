using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CyclopsBeaconDeployer.Items;
using HarmonyLib;
using Nautilus.Handlers;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CyclopsBeaconDeployer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.CyclopsBeaconDeployer";
        private const string pluginName = "Cyclops Beacon Deployer";
        private const string versionString = "1.1.4";

        public static ConfigEntry<bool> WriteLogs;

        private static readonly Harmony harmony = new Harmony(myGUID);
        public static ManualLogSource logger;

        public static string AssetFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static AssetBundle assetBundle { get; private set; }
        public static GameObject nameInputFieldGO { get; private set; }
        public static GameObject decoyPrefab { get; private set; }
        public static GameObject beaconPrefab { get; private set; }

        internal static JsonValue BeaconDecoyJsonEquipment
        {
            get
            {
                if (_beaconDecoyJsonEquipment == null)
                {
                    _beaconDecoyJsonEquipment = new JsonValue
                    {
                        {
                            TechData.propertyTechType,
                            new JsonValue((int)TechType.Beacon)
                        },
                        {
                            TechData.propertyEquipmentType,
                            new JsonValue((int)EquipmentType.DecoySlot)
                        }
                    };
                }

                return _beaconDecoyJsonEquipment;
            }
        }

        private static JsonValue _beaconDecoyJsonEquipment;
        
        internal static JsonValue BeaconNormalJsonEquipment
        {
            get
            {
                if (_beaconNormalJsonEquipment == null)
                {
                    _beaconNormalJsonEquipment = new JsonValue
                    {
                        {
                            TechData.propertyTechType,
                            new JsonValue((int)TechType.Beacon)
                        },
                        {
                            TechData.propertyEquipmentType,
                            new JsonValue((int)EquipmentType.Hand)
                        }
                    };
                }

                return _beaconNormalJsonEquipment;
            }
        }

        private static JsonValue _beaconNormalJsonEquipment;

        private IEnumerator Start()
        {
            logger = Logger;

            InitializeConfigs();

            CoroutineTask<GameObject> decoyTask = CraftData.GetPrefabForTechTypeAsync(TechType.CyclopsDecoy);
            yield return decoyTask;

            CoroutineTask<GameObject> beaconTask = CraftData.GetPrefabForTechTypeAsync(TechType.Beacon);
            yield return beaconTask;

            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetFolderPath, "beacondeployer"));
            nameInputFieldGO = assetBundle.LoadAsset<GameObject>("BeaconNameInput");

            decoyTask.result.value.AddComponent<PlaceTool>();
            decoyPrefab = decoyTask.result.value;

            beaconPrefab = beaconTask.result.value;
            
            TechData.entries[TechType.Beacon] = BeaconNormalJsonEquipment;

            new Deployer_ModOptions();
            BeaconDeployModule.Patch();
            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void InitializeConfigs()
        {
            WriteLogs = Config.Bind("Cyclops Beacon Deployer", "Write Logs", false);
        }
    }
}
