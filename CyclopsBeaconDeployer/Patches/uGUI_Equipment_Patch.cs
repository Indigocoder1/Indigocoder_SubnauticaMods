using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(uGUI_Equipment))]
    internal static class uGUI_Equipment_Patch
    {
        [HarmonyPatch(nameof(uGUI_Equipment.HighlightSlots)), HarmonyPostfix]
        private static void HighlightSlots(EquipmentType itemType, uGUI_Equipment __instance)
        {
            if (__instance.equipment == null)
            {
                return;
            }

            if (!Equipment_Patches.IsDecoyTube(__instance.equipment))
            {
                return;
            }

            __instance.equipment.GetSlots(itemType, uGUI_Equipment.sSlotIDs);
            for (int i = 0; i < uGUI_Equipment.sSlotIDs.Count; i++)
            {
                string key = uGUI_Equipment.sSlotIDs[i];
                uGUI_EquipmentSlot uGUI_EquipmentSlot;
                if (__instance.allSlots.TryGetValue(key, out uGUI_EquipmentSlot))
                {
                    Main_Plugin.logger.LogInfo($"uGUI_EquipmentSlot = {uGUI_EquipmentSlot.gameObject.name} | Key = {key}");
                    uGUI_EquipmentSlot.MarkCompatible(true);
                }
            }
            uGUI_Equipment.sSlotIDs.Clear();
        }
    }
}
