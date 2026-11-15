using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(InventoryItem))]
    public class InventoryItem_Patches
    {
        [HarmonyPatch(nameof(InventoryItem.isBindable)), HarmonyPostfix, HarmonyPatch(MethodType.Getter)]
        private static void isBindable_Postfix(InventoryItem __instance, ref bool __result)
        {
            if (__instance.techType != TechType.Beacon) return;

            __result = true;
        }
    }
}