using CyclopsBeaconDeployer.Items;
using HarmonyLib;
using System;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal static class Equipment_Patch
    {
        [HarmonyPatch(nameof(Equipment.AllowedToAdd)), HarmonyPostfix]
        public static void AllowedToAdd_Patch(Equipment __instance, Pickupable pickupable, ref bool __result)
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
            Main_Plugin.logger.LogInfo($"Has module = {hasModule}");
            if (pickupable.GetTechType() == TechType.Beacon)
            {
                __result = false;
                if (hasModule)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(nameof(Equipment.GetItemSlot)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(InventoryItem), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref})]
        private static void GetItemSlot_Patch(Equipment __instance, InventoryItem item, ref string slot)
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
            Main_Plugin.logger.LogInfo($"Has module = {hasModule}");
            if (item.techType == TechType.Beacon)
            {
                if (hasModule)
                {
                    slot = "CyclopsDecoy";
                }
                else
                {
                    slot = "Hand";
                }
            }
        }

        [HarmonyPatch(nameof(Equipment.RemoveItem)), HarmonyPrefix]
        [HarmonyPatch(new Type[] { typeof(Pickupable) }, new ArgumentType[] { ArgumentType.Normal})]
        private static bool RemoveItem_Patch(Equipment __instance, Pickupable pickupable, ref bool __result)
        {
            if (!__instance.owner)
            {
                return true;
            }

            CyclopsDecoyLoadingTube tube = __instance.owner.GetComponent<CyclopsDecoyLoadingTube>();
            if (!tube)
            {
                Main_Plugin.logger.LogInfo("Decoy tube not found. Returning!");
                return true;
            }

            if(pickupable.GetTechType() == TechType.Beacon)
            {
                string empty = string.Empty;
                __result = __instance.GetItemSlot(pickupable, ref empty) && __instance.RemoveItem(empty, true, true) != null;
                return false;
            }

            return true;
        }

        [HarmonyPatch("IItemsContainer.AllowedToRemove"), HarmonyPostfix]
        private static void AllowedToRemove_Patch(Equipment __instance, Pickupable pickupable, ref bool __result)
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

            if(pickupable.GetTechType() == TechType.Beacon)
            {
                __result = true;
            }
        }

        [HarmonyPatch("IItemsContainer.RemoveItem"), HarmonyPostfix]
        private static void RemoveItem_Patch(Equipment __instance, InventoryItem item, bool verbose, ref bool __result)
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

            if (item.techType == TechType.Beacon)
            {
                string empty = string.Empty;
                __instance.GetItemSlot(item, ref empty);
                __instance.RemoveItem(empty, true, verbose);
                __result = true;
            }
        }
    }
}