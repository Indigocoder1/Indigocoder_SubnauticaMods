using Chameleon.Monobehaviors;
using Chameleon.Monobehaviors.UI;
using HarmonyLib;
using System;
using UnityEngine;

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

        [HarmonyPatch(typeof(SubRoot)), HarmonyPatch(nameof(SubRoot.OnTakeDamage)), HarmonyPostfix]
        private static void OnTakeDamage_Postfix(SubRoot __instance, DamageInfo damageInfo)
        {
            if (__instance is not ChameleonSubRoot) return;

            switch (damageInfo.type)
            {
                case DamageType.Collide:
                    damageInfo.damage *= 1.5f;
                    float value = Mathf.Clamp(damageInfo.damage / 100f, 0.5f, 1.5f);
                    MainCameraControl.main.ShakeCamera(value);
                    break;
                case DamageType.Normal:
                    if (damageInfo.dealer == null) break;

                    if (damageInfo.dealer.TryGetComponent<Creature>(out _))
                    {
                        __instance.BroadcastMessage("OnTakeCreatureDamage", SendMessageOptions.DontRequireReceiver);
                    }
                    break;
            }
        }
    }
}
