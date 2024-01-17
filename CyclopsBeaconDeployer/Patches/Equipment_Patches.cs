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
                    __result = true;
                    /*
                    if (hasModule)
                    {
                        __result = false;
                    }
                    else
                    {
                        __result = true;
                    }
                    */
                }
            }
        }

        [HarmonyPatch(nameof(Equipment.GetItemSlot)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(Pickupable), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        private static void GetItemSlot_Pickupable_Patch(Equipment __instance, Pickupable pickupable, ref string slot)
        {
            if(pickupable.GetTechType() != TechType.Beacon)
            {
                return;
            }

            CyclopsDecoyLoadingTube tube;
            if (!IsDecoyTube(__instance, out tube))
            {
                return;
            }

            int slotNum = 1;
            try
            {
                slotNum = int.Parse(slot.Split('t')[1]);
            }
            catch (Exception e)
            {
                Main_Plugin.logger.LogError($"Error parsing decoy slot number! Message: \n{e.Message}");
            }

            SubRoot subroot = tube.transform.parent.GetComponent<SubRoot>();
            if (HasModule(subroot.upgradeConsole.modules))
            {
                slot = $"DecoySlot{slotNum}";
            }
            else
            {
                slot = "Hand";
            }
        }

        [HarmonyPatch(nameof(Equipment.GetItemSlot)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(InventoryItem), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref})]
        private static void GetItemSlot_Patch(Equipment __instance, InventoryItem item, ref string slot)
        {
            if(item.techType != TechType.Beacon)
            {
                return;
            }

            CyclopsDecoyLoadingTube tube;
            if (!IsDecoyTube(__instance, out tube))
            {
                return;
            }

            int slotNum = 1;
            try
            {
                slotNum = int.Parse(slot.Split('t')[1]);
            }
            catch (Exception e)
            {
                Main_Plugin.logger.LogError($"Error parsing decoy slot number! Message: \n{e.Message}");
            }

            SubRoot subroot = tube.transform.parent.GetComponent<SubRoot>();
            if (HasModule(subroot.upgradeConsole.modules))
            {
                slot = $"DecoySlot{slotNum}";
            }
            else
            {
                slot = "Hand";
            }
        }

        [HarmonyPatch(nameof(Equipment.NotifyEquip)), HarmonyPostfix]
        private static void NotifyEquip(Equipment __instance, InventoryItem item)
        {
            if(item.techType != BeaconDeployModule.techType)
            {
                return;
            }

            UpgradeConsole console = __instance.owner.GetComponent<UpgradeConsole>();
            if (!console)
            {
                return;
            }

            if (console.transform.parent.TryGetComponent<SubRoot>(out SubRoot subRoot))
            {
                CyclopsDecoyLauncher launcher = subRoot.transform.Find("DecoyLauncher")?.GetComponent<CyclopsDecoyLauncher>();
                launcher.decoyPrefab = Main_Plugin.beaconPrefab;

                GameObject nameInput = subRoot.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities/BeaconNameInput(Clone)")?.gameObject;
                if (nameInput)
                    nameInput.SetActive(true);
            }

            CraftData.equipmentTypes[TechType.Beacon] = EquipmentType.DecoySlot;
            //CraftData.equipmentTypes[TechType.CyclopsDecoy] = Main_Plugin.DecoyPlaceholder;
        }

        [HarmonyPatch(nameof(Equipment.NotifyUnequip)), HarmonyPostfix]
        private static void NotifyUnequip(Equipment __instance, InventoryItem item)
        {
            if (item.techType != BeaconDeployModule.techType)
            {
                return;
            }

            UpgradeConsole console = __instance.owner.GetComponent<UpgradeConsole>();
            if (!console)
            {
                return;
            }

            if (console.transform.parent.TryGetComponent<SubRoot>(out SubRoot subRoot))
            {
                CyclopsDecoyLauncher launcher = subRoot.transform.Find("DecoyLauncher").GetComponent<CyclopsDecoyLauncher>();
                launcher.decoyPrefab = Main_Plugin.decoyPrefab;

                GameObject nameInput = subRoot.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities/BeaconNameInput(Clone)")?.gameObject;
                if (nameInput)
                    nameInput.SetActive(false);
            }

            CraftData.equipmentTypes[TechType.Beacon] = EquipmentType.Hand;
            CraftData.equipmentTypes[TechType.CyclopsDecoy] = EquipmentType.DecoySlot;
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
    }
}