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
            //return !(__instance.storageRoot == null);
        }
    }
}
