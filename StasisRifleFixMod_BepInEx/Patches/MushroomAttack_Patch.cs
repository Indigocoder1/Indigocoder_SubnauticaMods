using HarmonyLib;

namespace StasisRifleFixMod_BepInEx
{
    [HarmonyPatch(typeof(MushroomAttack))]
    internal static class MushroomAttack_Patch
    {
        [HarmonyPatch(nameof(MushroomAttack.Evaluate)), HarmonyPostfix]
        private static void Patch(Creature creature, ref float __result)
        {
            if(creature.TryGetComponent<StasisRifleFix_Component>(out StasisRifleFix_Component comp))
            {
                if(comp.IsFrozen())
                {
                    __result = 0f;
                }
            }
        }
    }
}
