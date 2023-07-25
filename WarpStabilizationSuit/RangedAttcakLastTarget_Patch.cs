using HarmonyLib;
using IndigocoderLib.Utilities;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(RangedAttackLastTarget))]
    internal static class RangedAttcakLastTarget_Patch
    {
        [HarmonyPatch(nameof(RangedAttackLastTarget.Evaluate)), HarmonyPostfix]
        private static void Patch(Creature creature, ref float __result)
        {
            if(Utilities.GetNameWithCloneRemoved(creature.name).ToLower() != "warper")
            {
                return;
            }

            if (Inventory.main.equipment.GetCount(Suit_Craftable.itemTechType) > 0)
            {
                __result = 0;
            }
        }
    }
}
