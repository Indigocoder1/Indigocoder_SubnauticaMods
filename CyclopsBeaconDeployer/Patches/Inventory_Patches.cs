using HarmonyLib;
using System;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(Inventory))]
    internal static class Inventory_Patches
    {
        [HarmonyPatch(nameof(Inventory.AddOrSwap)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(InventoryItem), typeof(Equipment), typeof(string) })]
        private static void AddOrSwap_Patch(InventoryItem itemA, Equipment equipmentB, ref bool __result)
        {
            if(itemA.techType != TechType.Beacon)
            {
                return;
            }

            bool isTube = Equipment_Patches.IsDecoyTube(equipmentB, out CyclopsDecoyLoadingTube tube);
            if (!isTube)
            {
                return;
            }

            if(!Equipment_Patches.HasModule(equipmentB))
            {
                __result = false;
            }
        }

        [HarmonyPatch(nameof(Inventory.CanSwap)), HarmonyPostfix]
        private static void CanSwap(InventoryItem itemA, InventoryItem itemB, ref bool __result)
        {
            if(itemB == null)
            { 
                return;
            }

            IItemsContainer container = itemB.container;
            Equipment equipment = container as Equipment;

            if(equipment == null)
            {
                return;
            }

            if(itemA.techType == TechType.Beacon)
            {
                if(Equipment_Patches.HasModule(equipment))
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                }
            }

            if(itemA.techType == TechType.CyclopsDecoy)
            {
                if (Equipment_Patches.HasModule(equipment))
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
}
