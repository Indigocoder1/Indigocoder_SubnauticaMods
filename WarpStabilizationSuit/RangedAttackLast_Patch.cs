using HarmonyLib;
using static VFXParticlesPool;
using UnityEngine;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(RangedAttackLastTarget))]
    internal static class RangedAttackLast_Patch
    {
        [HarmonyPatch(nameof(RangedAttackLastTarget.Evaluate)), HarmonyPostfix]
        private static void Patch(Creature creature, ref float __result)
        {
            if(IndigocoderLib.Utilities.GetNameWithCloneRemoved(creature.name) == "Warper")
            {
                return;
            }

            bool hasSuit = Inventory.main.equipment.GetCount(Suit_Craftable.suitTechType) > 0;
            bool hasGloves = Inventory.main.equipment.GetCount(Gloves_Craftable.glovesTechType) > 0;

            if (hasSuit)
            {
                __result = 0.2f;
            }
        }
    }
}
