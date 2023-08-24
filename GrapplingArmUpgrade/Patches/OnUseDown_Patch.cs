using HarmonyLib;
using GrapplingArmUpgrade_BepInEx.Monobehaviours;

namespace GrapplingArmUpgrade_BepInEx.Patches
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    internal static class OnUseDown_Patch
    {
        [HarmonyPatch("IExosuitArm.OnUseDown"), HarmonyPostfix]
        private static void OnUseDown(ExosuitGrapplingArm __instance, ref float cooldownDuration)
        {
            if (__instance.GetComponent<UpgradedArm_Identifier>() == null)
            {
                return;
            }

            cooldownDuration = Main_Plugin.ArmCooldown.Value;
        }
    }
}
