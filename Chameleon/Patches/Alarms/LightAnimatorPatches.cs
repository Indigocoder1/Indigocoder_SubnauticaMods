using HarmonyLib;
using UnityEngine;

namespace Chameleon.Alarms.Patches
{
    [HarmonyPatch(typeof(LightAnimator))]
    internal class LightAnimatorPatches
    {
        [HarmonyPatch(nameof(LightAnimator.Start)), HarmonyPostfix]
        private static void Start_Postfix(LightAnimator __instance)
        {
            if(__instance.lightComponent == null)
            {
                __instance.lightComponent = __instance.GetComponent<Light>();
            }
        }
    }
}
