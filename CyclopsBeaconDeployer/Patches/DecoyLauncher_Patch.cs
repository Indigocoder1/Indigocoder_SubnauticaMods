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
                if(Main_Plugin.WriteLogs.Value)
                    Main_Plugin.logger.LogInfo("Returning because the module is not installed");

                return true;
            }

            Rigidbody rb = subroot.GetComponent<Rigidbody>();
            float yPos = rb.velocity.y <= 0.05? __instance.transform.position.y : __instance.transform.position.y - 15;
            Vector3 postion = new Vector3(__instance.transform.position.x, yPos, __instance.transform.position.z);
            Beacon beacon = Object.Instantiate(__instance.decoyPrefab, postion, Quaternion.identity).GetComponent<Beacon>();

            if (beacon)
            {
                if (Main_Plugin.WriteLogs.Value)
                    Main_Plugin.logger.LogInfo("Setting label info");

                string label = SubRoot_Patch.inputFields[subroot].text;
                if(string.IsNullOrEmpty(label))
                {
                    label = string.Format("{0} {1}", Language.main.Get("BeaconDefaultPrefix"), BeaconManager.GetCount() + 1);
                }
                beacon.label = label;
                beacon.beaconLabel.SetLabel(label);
                beacon.name = $"Beacon ({label})";

                SubRoot_Patch.inputFields[subroot].text = "";
            }

            return false;
        }
    }
}
