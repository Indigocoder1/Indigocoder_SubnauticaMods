using HarmonyLib;
using UnityEngine;

namespace StasisRifleFixMod_BepInEx
{
    [HarmonyPatch(typeof(Creature))]
    internal static class Creature_Start_Patch
    {
        [HarmonyPatch(nameof(Creature.Start)), HarmonyPostfix]
        private static void Patch(Creature __instance)
        {
            Rigidbody instanceRB = __instance.gameObject.GetComponentInChildren<Rigidbody>();

            if (instanceRB == null)
            {
                if(Main_Plugin.WriteLogs.Value)
                    Main_Plugin.logger.LogInfo($"Skipping fix component because {__instance.name} doesn't have a rigidbody");
                return;
            }

            StasisRifleFix_Component fixComponent = instanceRB.gameObject.EnsureComponent<StasisRifleFix_Component>();
            fixComponent.creature = __instance;
        }
    }
}
