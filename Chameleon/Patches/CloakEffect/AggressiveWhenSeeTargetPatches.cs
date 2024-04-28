using HarmonyLib;
using UnityEngine;
using System;

namespace Chameleon.Patches.CloakEffect
{
    [HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
    internal class AggressiveWhenSeeTargetPatches
    {
        [HarmonyPatch(nameof(AggressiveWhenSeeTarget.IsTargetValid)), HarmonyPatch(argumentTypes: new[] { typeof(GameObject) }), HarmonyPostfix]
        private static void IsTargetValid_Postfix(AggressiveWhenSeeTarget __instance, GameObject target, ref bool __result)
        {
            //Main_Plugin.logger.LogInfo($"{__instance.name} is checking if {target} is valid. Result is {__result}");
        }
    }
}
