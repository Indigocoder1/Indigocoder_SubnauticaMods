using HarmonyLib;
using SealSubMod.MonoBehaviours;
using UnityEngine;

namespace SealGrapplingTurret.Patches
{
    [HarmonyPatch(typeof(SealSubRoot))]
    internal static class SealSubRoot_Patches
    {
        [HarmonyPatch(nameof(SealSubRoot.Start)), HarmonyPostfix]
        private static void Start_Patch(SealSubRoot __instance) 
        {
            Transform modelRoot = __instance.transform.Find("Scaler/SealSubModelPrefab/Seal Sub");
            Transform barrel = modelRoot.Find("Cylinder.006");
            Transform hook = GameObject.Instantiate(Main_Plugin.hookPrefab, barrel).transform;
            hook.localPosition = new Vector3(0, 0, -1.88f);
            hook.eulerAngles = new Vector3(0, 90f, 0);
            hook.localScale = new Vector3(0.5f, 2, 2);
        }
    }
}
