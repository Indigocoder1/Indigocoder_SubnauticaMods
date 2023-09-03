using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CraftData))]
    internal static class CraftData_Patches
    {
        [HarmonyPatch(nameof(CraftData.GetQuickSlotType)), HarmonyPostfix]
        private static void GetQuickSlotType_Patch(TechType techType, ref QuickSlotType __result)
        {
            if(techType == TechType.Beacon)
            {
                __result = QuickSlotType.Selectable;
            }
        }
    }
}
