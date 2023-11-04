using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(InventoryItem))]
    internal static class InventoryItemPatch
    {
        [HarmonyPatch(nameof(InventoryItem.isBindable), MethodType.Getter), HarmonyPostfix]
        private static void InventoryItem_Patch(InventoryItem __instance, ref bool __result)
        {
            __result = false;

            if (CraftData.GetEquipmentType(__instance.techType) == EquipmentType.Hand)
            {
                __result = true;
            }

            /*
            if(CraftData.GetEquipmentType(__instance.techType) == EquipmentType.DecoySlot)
            {
                __result = true;
            }
            */
        }
    }
}
