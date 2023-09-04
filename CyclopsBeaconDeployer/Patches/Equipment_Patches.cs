using CyclopsBeaconDeployer.Items;
using HarmonyLib;
using IndigocoderLib;
using System;
using System.Net;
using UnityEngine;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal static class Equipment_Patches
    {
        [HarmonyPatch(nameof(Equipment.AllowedToAdd)), HarmonyPostfix]
        public static void AllowedToAdd_Patch(Equipment __instance, Pickupable pickupable, ref bool __result)
        {
            if (IsDecoyTube(__instance, out CyclopsDecoyLoadingTube tube))
            {
                bool hasModule = tube.subRoot.upgradeConsole.modules.GetCount(BeaconDeployModule.techType) > 0;
                Main_Plugin.logger.LogInfo($"Has module = {hasModule}");
                if (pickupable.GetTechType() == TechType.Beacon)
                {
                    if (hasModule)
                    {
                        __result = true;
                    }
                    else
                    {
                        __result = false;
                    }
                }

                if(pickupable.GetTechType() == TechType.CyclopsDecoy)
                {
                    if (hasModule)
                    {
                        __result = false;
                    }
                    else
                    {
                        __result = true;
                    }
                }
            }
        }

        [HarmonyPatch(nameof(Equipment.GetItemSlot)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(Pickupable), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        private static void GetItemSlot_Pickupable_Patch(Equipment __instance, Pickupable pickupable, ref string slot)
        {
            Main_Plugin.logger.LogInfo($"Original slot = {slot}");

            if (IsDecoyTube(__instance, out CyclopsDecoyLoadingTube tube))
            {
                int slotNum = 1;
                try
                {
                    slotNum = int.Parse(slot.Split('t')[1]);
                }
                catch (Exception e)
                {
                    Main_Plugin.logger.LogError($"Error parsing decoy slot number! Message: \n{e.Message}");
                }

                if (pickupable.GetTechType() == TechType.Beacon)
                {
                    if (HasModule(__instance))
                    {
                        slot = $"DecoySlot{slotNum}";
                    }
                    else
                    {
                        slot = "Hand";
                    }
                }
            }            
        }

        [HarmonyPatch(nameof(Equipment.GetItemSlot)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(InventoryItem), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref})]
        private static void GetItemSlot_Patch(Equipment __instance, InventoryItem item, ref string slot)
        {
            Main_Plugin.logger.LogInfo($"Original slot = {slot}");

            if (IsDecoyTube(__instance, out CyclopsDecoyLoadingTube tube))
            {
                int slotNum = 1;
                try
                {
                    slotNum = int.Parse(slot.Split('t')[1]);
                }
                catch (Exception e)
                {
                    Main_Plugin.logger.LogError($"Error parsing decoy slot number! Message: \n{e.Message}");
                }

                if (item.techType == TechType.Beacon)
                {
                    if (HasModule(__instance))
                    {
                        slot = $"DecoySlot{slotNum}";
                    }
                    else
                    {
                        slot = "Hand";
                    }
                }
            }
        }

        [HarmonyPatch(nameof(Equipment.RemoveItem)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(Pickupable) }, new ArgumentType[] { ArgumentType.Normal})]
        private static void RemoveItem_Patch(Equipment __instance, Pickupable pickupable, ref bool __result)
        {
            if (IsDecoyTube(__instance, out CyclopsDecoyLoadingTube tube))
            {
                if (pickupable.GetTechType() == TechType.Beacon)
                {
                    string empty = string.Empty;
                    __instance.GetItemSlot(pickupable, ref empty);
                    __instance.RemoveItem(empty, true, true);
                    __result = true;
                }
            }            
        }

        [HarmonyPatch(nameof(Equipment.NotifyEquip)), HarmonyPostfix]
        private static void NotifyEquip(Equipment __instance, InventoryItem item)
        {
            if(item.techType != BeaconDeployModule.techType)
            {
                return;
            }

            if (__instance.owner.TryGetComponent<UpgradeConsole>(out UpgradeConsole console))
            {
                if (console.transform.parent.TryGetComponent<SubRoot>(out SubRoot subRoot))
                {
                    CyclopsDecoyLauncher launcher = subRoot.transform.Find("DecoyLauncher").GetComponent<CyclopsDecoyLauncher>();
                    launcher.decoyPrefab = Main_Plugin.beaconPrefab;
                    GameObject nameInput = subRoot.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities/BeaconNameInput(Clone)").gameObject;
                    nameInput.SetActive(true);
                }
            }
        }

        [HarmonyPatch(nameof(Equipment.NotifyUnequip)), HarmonyPostfix]
        private static void NotifyUnequip(Equipment __instance, InventoryItem item)
        {
            if (item.techType != BeaconDeployModule.techType)
            {
                return;
            }

            if (__instance.owner.TryGetComponent<UpgradeConsole>(out UpgradeConsole console))
            {
                if(console.transform.parent.TryGetComponent<SubRoot>(out SubRoot subRoot))
                {
                    CyclopsDecoyLauncher launcher = subRoot.transform.Find("DecoyLauncher").GetComponent<CyclopsDecoyLauncher>();
                    launcher.decoyPrefab = Main_Plugin.decoyPrefab;
                    GameObject nameInput = subRoot.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities/BeaconNameInput(Clone)").gameObject;
                    nameInput.SetActive(false);
                }
            }
        }

        public static bool IsDecoyTube(Equipment equipment, out CyclopsDecoyLoadingTube decoyTube)
        {
            decoyTube = null;
            if (equipment.owner == null)
            {
                return false;
            }

            CyclopsDecoyLoadingTube tube = equipment.owner.GetComponent<CyclopsDecoyLoadingTube>();
            if (!tube)
            {
                return false;
            }

            decoyTube = tube;
            return true;
        }

        public static bool IsDecoyTube(Equipment equipment)
        {
            if (equipment.owner == null)
            {
                return false;
            }

            CyclopsDecoyLoadingTube tube = equipment.owner.GetComponent<CyclopsDecoyLoadingTube>();
            if (!tube)
            {
                return false;
            }

            return true;
        }

        public static bool HasModule(Equipment equipment)
        {
            return equipment.GetCount(BeaconDeployModule.techType) > 0;
        }

        public static bool HasModule(CyclopsDecoyLoadingTube tube)
        {
            return tube.subRoot.upgradeConsole.modules.GetCount(BeaconDeployModule.techType) > 0;
        }
    }
}