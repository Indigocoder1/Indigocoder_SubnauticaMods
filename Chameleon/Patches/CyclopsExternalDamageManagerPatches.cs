using Chameleon.Monobehaviors;
using HarmonyLib;
using System;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(CyclopsExternalDamageManager))]
    internal class CyclopsExternalDamageManagerPatches
    {
        [HarmonyPatch(nameof(CyclopsExternalDamageManager.OnEnable)), HarmonyPrefix]
        private static bool OnEnable_Prefix(CyclopsExternalDamageManager __instance)
        {
            bool isSubRootSeal = false;
            if(Main_Plugin.SealSubInstalled)
            {
                isSubRootSeal = __instance.subRoot.GetType() == Type.GetType("SealSubRoot");
            }

            return !(isSubRootSeal || __instance.subRoot is ChameleonSubRoot);
        }
    }
}
