using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyManager))]
    internal static class DecoyManager_Patches
    {
        private static Dictionary<CyclopsDecoyManager, GameObject> previousPrefabs = new Dictionary<CyclopsDecoyManager, GameObject>();

        [HarmonyPatch(nameof(CyclopsDecoyManager.TryLaunchDecoy)), HarmonyPrefix]
        private static void TryLaunchDecoy_Prefix(CyclopsDecoyManager __instance)
        {
            Equipment decoySlots = __instance.decoyLoadingTube.decoySlots;
            int num = 0;
            string slot = "";
            for (int i = 1; i <= 5; i++)
            {
                string text = "DecoySlot" + i.ToString();
                if (decoySlots.GetTechTypeInSlot(text) != TechType.None)
                {
                    slot = text;
                    num++;
                }
            }

            previousPrefabs[__instance] = __instance.decoyLauncher.decoyPrefab;
            if (num > 0)
            {
                if(decoySlots.GetTechTypeInSlot(slot) == TechType.CyclopsDecoy)
                {
                    __instance.decoyLauncher.decoyPrefab = Main_Plugin.decoyPrefab;
                }
                else if(decoySlots.GetTechTypeInSlot(slot) == TechType.Beacon)
                {
                    __instance.decoyLauncher.decoyPrefab = Main_Plugin.beaconPrefab;
                }
            }
        }

        [HarmonyPatch(nameof(CyclopsDecoyManager.LaunchWithDelay)), HarmonyPostfix]
        private static void LaunchWithDelay_Postfix(CyclopsDecoyManager __instance)
        {
            __instance.decoyLauncher.decoyPrefab = previousPrefabs[__instance];
        }

        [HarmonyPatch(nameof(CyclopsDecoyManager.UpdateTotalDecoys)), HarmonyPostfix]
        private static void UpdateTotalDecoys_Postfix(CyclopsDecoyManager __instance)
        {
            SubRoot subRoot = __instance.GetComponent<SubRoot>();

            if(SubRoot_Patch.inputFields.ContainsKey(subRoot))
            {
                SubRoot_Patch.inputFields[subRoot].interactable = __instance.decoyCount > 0 ? true : false;
            }
            
        }
    }
}
