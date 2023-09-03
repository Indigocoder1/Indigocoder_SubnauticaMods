using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(Inventory))]
    internal static class Inventory_Patch
    {
        /*
        [HarmonyPatch(nameof(Inventory.AddOrSwap)), HarmonyPostfix]
        private static void AddOrSwap_Patch(InventoryItem itemA, Equipment equipmentB, ref bool __result)
        {
            if(itemA.techType != TechType.Beacon)
            {
                return;
            }

            bool isTube = Equipment_Patch.IsDecoyTube(equipmentB, out CyclopsDecoyLoadingTube tube);
            if (!isTube)
            {
                return;
            }

            if(!Equipment_Patch.HasModule(equipmentB))
            {
                __result = false;
            }
        }
        */
    }
}
