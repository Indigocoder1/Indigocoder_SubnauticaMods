using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using UnityEngine;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(Gravsphere))]
    internal static class ObjectType_Patches
    {
        [HarmonyPatch(nameof(Gravsphere.Start)), HarmonyPostfix]
        private static void Start_Postfix(Gravsphere __instance)
        {
            __instance.gameObject.EnsureComponent<GravTrapObjectsType>();
        }

        [HarmonyPatch(nameof(Gravsphere.AddAttractable)), HarmonyPostfix]
        static void Gravsphere_AddAttractable_Postfix(Gravsphere __instance, Rigidbody r)
        {
            __instance.GetComponent<GravTrapObjectsType>().HandleAttracted(r.gameObject, true);
        }

        [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.DestroyEffect)), HarmonyPostfix]
        static void Gravsphere_DestroyEffect_Postfix(Gravsphere __instance, int index)
        {
            var rigidBody = __instance.attractableList[index];
            if (rigidBody)
                __instance.GetComponent<GravTrapObjectsType>().HandleAttracted(rigidBody.gameObject, false);
        }

        [HarmonyPatch(typeof(Gravsphere), nameof(Gravsphere.IsValidTarget)), HarmonyPostfix]
        static void Gravsphere_IsValidTarget_Prefix(Gravsphere __instance, GameObject obj, ref bool __result)
        {
            if(__instance.GetComponent<EnhancedGravSphere>() == null)
            {
                return;
            }

            __result = __instance.GetComponent<GravTrapObjectsType>().IsValidTarget(obj);
        }
    }
}
