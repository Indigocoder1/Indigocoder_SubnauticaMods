using GrapplingArmUpgrade_BepInEx.Monobehaviours;
using HarmonyLib;
using UnityEngine;

namespace GrapplingArmUpgrade_BepInEx.Patches
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_Patches
    {
        [HarmonyPatch(nameof(Exosuit.GetArmPrefab)), HarmonyPostfix]
        private static void GetArmPrefab_Patch(TechType techType, Exosuit __instance, ref GameObject __result)
        {
            if (techType != UpgradedArm_Craftable.TechType)
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

            Main_Plugin.logger.LogInfo($"Exosuit upgraded grappling arm prefab name = {result.name}");

            result.EnsureComponent<ExosuitGrapplingArm>();
            __result = result;
        }

        [HarmonyPatch(nameof(Exosuit.OnUpgradeModuleChange)), HarmonyPrefix]
        private static bool OnModuleChanged_Patch(Exosuit __instance, TechType techType)
        {
            if (techType != UpgradedArm_Craftable.TechType)
            {
                return true;
            }

            __instance.MarkArmsDirty();
            return false;
        }

        [HarmonyPatch(nameof(Exosuit.SpawnArm)), HarmonyPrefix]
        private static bool SpawnArm_Patch(Exosuit __instance, TechType techType, Transform parent, ref IExosuitArm __result)
        {
            if (techType != UpgradedArm_Craftable.TechType)
            {
                return true;
            }
            __result = __instance.SpawnArm(TechType.ExosuitGrapplingArmModule, parent);
            __result.GetGameObject().AddComponent<UpgradedArm_Identifier>();
            return false;
        }

        [HarmonyPatch(nameof(Exosuit.UpdateExosuitArms)), HarmonyPostfix]
        private static void UpdateArms_Patch(Exosuit __instance)
        {
            TechType slotBinding = __instance.GetSlotBinding(__instance.GetSlotIndex("ExosuitArmLeft"));
            TechType slotBinding2 = __instance.GetSlotBinding(__instance.GetSlotIndex("ExosuitArmRight"));

            if (slotBinding == UpgradedArm_Craftable.TechType)
            {
                __instance.grapplingArms.Add(__instance.leftArm as ExosuitGrapplingArm);
            }
            else if (slotBinding2 == UpgradedArm_Craftable.TechType)
            {
                __instance.grapplingArms.Add(__instance.rightArm as ExosuitGrapplingArm);
            }
        }
    }
}
