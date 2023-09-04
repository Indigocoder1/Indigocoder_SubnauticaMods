using HarmonyLib;
using UnityEngine;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyLauncher))]
    internal static class DecoyLauncher_Patch
    {
        [HarmonyPatch(nameof(CyclopsDecoyLauncher.LaunchDecoy)), HarmonyPrefix]
        private static bool LaunchDecoy(CyclopsDecoyLauncher __instance)
        {
            SubRoot subroot = __instance.transform.parent.GetComponent<SubRoot>();
            if(!Equipment_Patches.HasModule(subroot.upgradeConsole.modules))
            {
                Main_Plugin.logger.LogInfo("Returning because the module is not installed");
                return true;
            }

            Beacon beacon = Object.Instantiate(__instance.decoyPrefab, __instance.transform.position, Quaternion.identity).GetComponent<Beacon>();
            Main_Plugin.logger.LogInfo($"Beacon = {beacon}");

            if (beacon)
            {
                Main_Plugin.logger.LogInfo("Setting label info");
                string label = SubRoot_Patch.inputFields[subroot].text;
                if(string.IsNullOrEmpty(label))
                {
                    label = string.Format("{0} {1}", Language.main.Get("BeaconDefaultPrefix"), BeaconManager.GetCount() + 1);
                }
                beacon.label = label;
                beacon.beaconLabel.SetLabel(label);
                beacon.name = $"Beacon ({label})";
            }

            return false;
        }
    }
}
