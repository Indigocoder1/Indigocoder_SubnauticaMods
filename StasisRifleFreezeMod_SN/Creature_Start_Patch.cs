using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using UnityEngine;

namespace StasisRifleFix
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
                if (QMod.Config.writeLogs)
                    Logger.Log(Logger.Level.Info, $"Skipping fix component because {__instance.name} doesn't have a rigidbody");
                return;
            }

            instanceRB.gameObject.EnsureComponent<StasisRifleFix_Component>();
        }
    }
}
