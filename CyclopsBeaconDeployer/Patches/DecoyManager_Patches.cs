using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static VehicleUpgradeConsoleInput;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyManager))]
    internal static class DecoyManager_Patches
    {
        public static Dictionary<SubRoot, DecoyInfo> decoyInfos = new Dictionary<SubRoot, DecoyInfo>();
        private static Dictionary<CyclopsDecoyManager, GameObject> previousPrefab = new Dictionary<CyclopsDecoyManager, GameObject>();

        [HarmonyPatch(nameof(CyclopsDecoyManager.TryLaunchDecoy)), HarmonyPrefix]
        private static void TryLaunchDecoy_Prefix(CyclopsDecoyManager __instance)
        {
            Equipment decoySlots = __instance.decoyLoadingTube.decoySlots;
            int decoyCount = 0;
            int beaconCount = 0;
            string slotWithDecoy = "";
            string slotWithBeacon = "";
            for (int i = 1; i <= 5; i++)
            {
                string text = "DecoySlot" + i.ToString();
                TechType typeInSlot = decoySlots.GetTechTypeInSlot(text);

                if (typeInSlot == TechType.Beacon)
                {
                    slotWithBeacon = text;
                    beaconCount++;
                }

                if(typeInSlot == TechType.CyclopsDecoy)
                {
                    slotWithDecoy = text;
                    decoyCount++;
                }
            }

            bool textInField = !string.IsNullOrEmpty(SubRoot_Patch.inputFields[__instance.subRoot].text);
            bool launchingBeacon = false;

            Main_Plugin.logger.LogInfo($"Beacon count = {beaconCount}");
            Main_Plugin.logger.LogInfo($"Decoy count = {decoyCount}");

            previousPrefab[__instance] = __instance.decoyLauncher.decoyPrefab;
            if (beaconCount + decoyCount > 0)
            {
                if(textInField && beaconCount > 0)
                {
                    __instance.decoyLauncher.decoyPrefab = Main_Plugin.beaconPrefab;
                    launchingBeacon = true;
                }
                else
                {
                    __instance.decoyLauncher.decoyPrefab = Main_Plugin.decoyPrefab;
                }
            }

            Main_Plugin.logger.LogInfo($"Launching beacon = {launchingBeacon}");
            Main_Plugin.logger.LogInfo($"Slot with beacon = {slotWithBeacon}");
            Main_Plugin.logger.LogInfo($"Slot with decoy = {slotWithDecoy}");
            if (!decoyInfos.ContainsKey(__instance.subRoot))
            {
                decoyInfos.Add(__instance.subRoot, new DecoyInfo(launchingBeacon, slotWithDecoy, slotWithBeacon));
            }
            else
            {
                decoyInfos[__instance.subRoot] = new DecoyInfo(launchingBeacon, slotWithDecoy, slotWithBeacon);
            }
        }

        [HarmonyPatch(nameof(CyclopsDecoyManager.LaunchWithDelay)), HarmonyPostfix]
        private static void LaunchWithDelay_Postfix(CyclopsDecoyManager __instance)
        {
            bool textInField = !string.IsNullOrEmpty(SubRoot_Patch.inputFields[__instance.subRoot].text);
            Main_Plugin.logger.LogInfo($"Text in field = {textInField}");
            __instance.decoyLauncher.decoyPrefab = previousPrefab[__instance];
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

        public struct DecoyInfo
        {
            public bool launchedBeacon;
            public string slotWithDecoy;
            public string slotWithBeacon;

            public DecoyInfo(bool launchedBeacon, string slotWithDecoy, string slotWithBeacon)
            {
                this.launchedBeacon = launchedBeacon;
                this.slotWithDecoy = slotWithDecoy;
                this.slotWithBeacon = slotWithBeacon;
            }
        }
    }
}
