using HarmonyLib;
using UnityEngine;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(EnergyMixin))]
    internal class EnergyMixinPatches
    {
        [HarmonyPatch(nameof(EnergyMixin.Initialize)), HarmonyPrefix]
        private static void Initialiation_Prefix(EnergyMixin __instance)
        {
            //__instance.storageRoot = __instance.GetComponentInChildren<ChildObjectIdentifier>();
        }
    }
}
