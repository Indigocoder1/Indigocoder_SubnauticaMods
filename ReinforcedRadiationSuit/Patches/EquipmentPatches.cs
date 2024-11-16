using HarmonyLib;
using ReinforcedRadiationSuit.Items;
using UnityEngine;

namespace ReinforcedRadiationSuit.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal static class EquipmentPatches
    {
        [HarmonyPatch(nameof(Equipment.GetCount)), HarmonyPostfix]
        private static void GetCount_Patch(Equipment __instance, TechType techType, ref int __result)
        {
            if (Inventory.main.equipment != __instance) return;

            __result = techType switch
            {
                TechType.Rebreather => Mathf.Max(__result, __instance.GetCount(RebreatherRadiationHelmet_Craftable.techType)),
                TechType.RadiationSuit => __result = Mathf.Max(__result, __instance.GetCount(ReinforcedRadiationSuit_Craftable.techType)),
                TechType.RadiationGloves => __result = Mathf.Max(__result, __instance.GetCount(ReinforcedRadiationGloves_Craftable.techType)),
                TechType.RadiationHelmet => __result = Mathf.Max(__result, __instance.GetCount(RebreatherRadiationHelmet_Craftable.techType)),
                _ => __result
            };
        }
    }
}
