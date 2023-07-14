using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using static Exosuit;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    internal static class ExosuitGrapplingArm_Start_Patch
    {
        [HarmonyPatch(nameof(ExosuitGrapplingArm.Start)), HarmonyPostfix]
        private static void Start_Patch(ExosuitGrapplingArm __instance)
        {
            ExosuitGrapplingArm instance = __instance;
            Main_Plugin.logger.LogInfo($"Grappling arm instance = {instance}");

            if (instance.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) < 1)
            {
                return;
            }

            instance.hook.GetComponent<SphereCollider>().radius = 0.25f;
            instance.hook.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}
