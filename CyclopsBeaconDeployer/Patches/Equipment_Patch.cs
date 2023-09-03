using CyclopsBeaconDeployer.Items;
using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal static class Equipment_Patch
    {
        [HarmonyPatch(nameof(Equipment.AllowedToAdd)), HarmonyPostfix]
        private static void AllowedToAdd_Patch(Equipment __instance, Pickupable pickupable, ref bool __result)
        {
            if(!__instance.owner)
            {
                return;
            }

            CyclopsDecoyLoadingTube tube = __instance.owner.GetComponent<CyclopsDecoyLoadingTube>();
            if (!tube)
            {
                Main_Plugin.logger.LogInfo("Decoy tube not found. Returning!");
                return;
            }

            bool hasModule = tube.subRoot.upgradeConsole.modules.GetCount(BeaconDeployModule.techType) > 0;
            Main_Plugin.logger.LogInfo($"Tube = {tube}");
            Main_Plugin.logger.LogInfo($"Has module = {hasModule}");
            if (pickupable.GetTechType() == TechType.Beacon)
            {
                __result = false;
                /*
                if(hasModule)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                }
                */
            }
        }

        [HarmonyPatch(nameof(Equipment.GetItemSlot)), HarmonyPostfix]
        private static void GetItemSlot_Patch(Equipment __instance, Pickupable pickupable, ref string slot)
        {
            if (!__instance.owner)
            {
                return;
            }

            CyclopsDecoyLoadingTube tube = __instance.owner.GetComponent<CyclopsDecoyLoadingTube>();
            if (!tube)
            {
                Main_Plugin.logger.LogInfo("Decoy tube not found. Returning!");
                return;
            }

            bool hasModule = tube.subRoot.upgradeConsole.modules.GetCount(BeaconDeployModule.techType) > 0;
            if (pickupable.GetTechType() == TechType.Beacon)
            {

                /*
                if (hasModule)
                {
                    slot = "CyclopsDecoy";
                }
                else
                {
                    slot = "Hand";
                }
                */
            }
        }
    }
}
