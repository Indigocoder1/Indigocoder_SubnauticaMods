using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    internal static class GrapplingArm_OnUseDown_Patch
    {
        [HarmonyPatch("IExosuitArm.OnUseDown"), HarmonyPostfix]
        private static void OnUseDown_Patch(ExosuitGrapplingArm __instance, ref float cooldownDuration)
        {
            if (__instance.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) < 1)
            {
                return;
            }

            cooldownDuration = 0.5f;
        }
    }
}
