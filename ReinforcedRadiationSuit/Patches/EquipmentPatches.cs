using HarmonyLib;
using ReinforcedRadiationSuit.Items;
using System.Collections.Generic;
using System.Linq;

namespace ReinforcedRadiationSuit.Patches
{
    /*
    [HarmonyPatch(typeof(Equipment))]
    internal static class EquipmentPatches
    {
        [HarmonyPatch(nameof(Equipment.GetCount)), HarmonyPostfix]
        private static void GetCount_Patch(Equipment __instance, TechType techType, ref int __result)
        {
            if (Inventory.main.equipment != __instance) return;

            if(techType == TechType.Rebreather && __instance.GetCount(RebreatherRadiationHelmet_Craftable.techType) != 0)
            {
                __result = __instance.GetCount(RebreatherRadiationHelmet_Craftable.techType);
            }

            if(SuitToRadiationTechTypes.Keys.Contains(techType) && __instance.GetCount(techType) != 0)
            {
                __result = __instance.GetCount(SuitToRadiationTechTypes[techType]);
            }
        }

        private static Dictionary<TechType, TechType> SuitToRadiationTechTypes = new()
        {
            { TechType.RadiationSuit, ReinforcedRadiationSuit_Craftable.techType },
            { TechType.RadiationGloves, ReinforcedRadiationGloves_Craftable.techType  },
            { TechType.RadiationHelmet, RebreatherRadiationHelmet_Craftable.techType }
        };
    }
    */
}
