using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_GetArmPrefab_Patch
    {
        [HarmonyPatch(nameof(Exosuit.GetArmPrefab)), HarmonyPostfix]
        private static void GetArmPrefab_Patch(TechType techType, Exosuit __instance, ref GameObject __result)
        {
            if(techType == TechType.ExosuitGrapplingArmModule)
            {
                Main_Plugin.logger.Log(LogLevel.Info, $"Exosuit grappling arm prefab name = {__result.name}");
            }

            if(techType != GrapplingArmUpgradeModule.TechType)
            {
                return;
            }

            GameObject result = null;
            for (int i = 0; i < __instance.armPrefabs.Length; i++)
            {
                if (__instance.armPrefabs[i].techType == TechType.ExosuitGrapplingArmModule)
                {
                    result = __instance.armPrefabs[i].prefab;
                    break;
                }
            }

            Main_Plugin.logger.Log(LogLevel.Info, $"Exosuit upgraded grappling arm prefab name = {result.name}");

            result.EnsureComponent<ExosuitGrapplingArm>();
            __result = result;
        }
    }
}
