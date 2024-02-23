using HarmonyLib;
using ReinforcedRadiationSuit.Items;
using System.Collections.Generic;

namespace ReinforcedRadiationSuit.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal static class Equipment_Patches
    {
        [HarmonyPatch(nameof(Equipment.GetCount)), HarmonyPostfix]
        private static void GetCount_Patch(Equipment __instance, TechType techType, ref int __result)
        {
            if (Inventory.main.equipment != __instance) return;

            if (techType != ReinforcedRadiationSuit_Craftable.techType) return;

            __result = SuitToRadiationTechTypes[techType];
        }

        private static Dictionary<TechType, TechType> SuitToRadiationTechTypes = new()
        {
            { ReinforcedRadiationSuit_Craftable.techType, TechType.RadiationSuit},

        }
    }
}
