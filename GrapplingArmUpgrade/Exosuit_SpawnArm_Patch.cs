using HarmonyLib;
using UnityEngine;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_SpawnArm_Patch
    {
        [HarmonyPatch(nameof(Exosuit.SpawnArm)), HarmonyPrefix]
        private static bool Patch(Exosuit __instance, TechType techType, Transform parent, ref IExosuitArm __result)
        {
            if (techType != GrapplingArmUpgradeModule.TechType)
            {
                return true;
            }
            __result = __instance.SpawnArm(TechType.ExosuitGrapplingArmModule, parent);
            __result.GetGameObject().AddComponent<GrapplingArmUpgraded>();
            return false;
        }
    }
}
