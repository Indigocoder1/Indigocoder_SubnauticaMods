using HarmonyLib;
using UnityEngine;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(EnergyMixin))]
    internal class EnergyMixinPatches
    {
        [HarmonyPatch(nameof(EnergyMixin.Initialize)), HarmonyPrefix]
        private static void Initialization_Prefix(EnergyMixin __instance)
        {
            Main_Plugin.logger.LogInfo($"Energy mixin parent = {__instance?.transform.parent}");
            Main_Plugin.logger.LogInfo($"Energy mixin = {__instance}");
            Main_Plugin.logger.LogInfo($"Storage root from Initialize = {__instance.storageRoot?.transform}");
            //return !(__instance.storageRoot == null);
        }
    }
}
