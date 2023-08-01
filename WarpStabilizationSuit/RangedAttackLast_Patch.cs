using HarmonyLib;
using static VFXParticlesPool;
using UnityEngine;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(RangedAttackLastTarget))]
    internal static class RangedAttackLast_Patch
    {
        [HarmonyPatch(nameof(RangedAttackLastTarget.Evaluate)), HarmonyPostfix]
        private static void Patch(ref float __result)
        {
            bool hasSuit = Inventory.main.equipment.GetCount(Suit_Craftable.suitTechType) > 0;
            bool hasGloves = Inventory.main.equipment.GetCount(Gloves_Craftable.glovesTechType) > 0;

            if (hasSuit)
            {
                __result = 0.2f;
            }
        }
    }
}
