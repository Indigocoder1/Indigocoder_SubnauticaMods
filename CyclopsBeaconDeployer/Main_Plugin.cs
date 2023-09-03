using BepInEx;
using BepInEx.Logging;
using CyclopsBeaconDeployer.Items;
using HarmonyLib;
using IndigocoderLib;
using System.Collections;
using UnityEngine;

namespace CyclopsBeaconDeployer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.CyclopsBeaconDeployer";
        private const string pluginName = "Cyclops Beacon Deployer";
        private const string versionString = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);
        public static ManualLogSource logger;

        private IEnumerator Start()
        {
            logger = Logger;

            if (PiracyDetector.TryFindPiracy())
            {
                yield break;
            }

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.CyclopsDecoy);
            yield return task;

            task.result.value.AddComponent<PlaceTool>();
            CraftData.equipmentTypes[TechType.Beacon] = EquipmentType.DecoySlot;

            BeaconDeployModule.Patch();
            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
