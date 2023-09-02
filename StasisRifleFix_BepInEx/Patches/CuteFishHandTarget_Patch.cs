using HarmonyLib;
using ProtoBuf.Meta;
using UnityEngine;

namespace StasisRifleFixMod_BepInEx
{
    [HarmonyPatch(typeof(CuteFishHandTarget))]
    internal static class CuteFishHandTarget_Patch
    {
        [HarmonyPatch(nameof(CuteFishHandTarget.AllowedToInteract)), HarmonyPostfix]
        private static void Patch(CuteFishHandTarget __instance, ref bool __result)
        {
            if (!CreatureIsFrozen(__instance.creatureRigidbody))
            {
                return;
            }
            else
            {
                __result = false;
            }
        }
        private static bool CreatureIsFrozen(Rigidbody instanceRB)
        {
            if (instanceRB == null)
            {
                return false;
            }

            if (StasisRifle.sphere.targets.Contains(instanceRB))
            {
                return true;
            }

            return false;
        }
    }
}
