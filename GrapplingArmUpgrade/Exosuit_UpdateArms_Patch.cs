using HarmonyLib;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_UpdateArms_Patch
    {
        [HarmonyPatch(nameof(Exosuit.UpdateExosuitArms)), HarmonyPostfix]
        private static void Patch(Exosuit __instance)
        {
            TechType slotBinding = __instance.GetSlotBinding(__instance.GetSlotIndex("ExosuitArmLeft"));
            TechType slotBinding2 = __instance.GetSlotBinding(__instance.GetSlotIndex("ExosuitArmRight"));

            if(slotBinding == GrapplingArmUpgradeModule.TechType)
            {
                __instance.grapplingArms.Add(__instance.leftArm as ExosuitGrapplingArm);
            }
            else if(slotBinding2 == GrapplingArmUpgradeModule.TechType)
            {
                __instance.grapplingArms.Add(__instance.rightArm as ExosuitGrapplingArm);
            }
        }
    }
}
