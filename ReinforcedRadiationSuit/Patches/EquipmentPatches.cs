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

            if(techType == TechType.Rebreather)
            {
                __result = Mathf.Max(__result, __instance.GetCount(RebreatherRadiationHelmet_Craftable.techType));
            }
        }
    }
}
