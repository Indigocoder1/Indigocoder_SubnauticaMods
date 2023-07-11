using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using System.Collections.Generic;

namespace StasisRifleFixMod_BepInEx
{
    [HarmonyPatch(typeof(Creature))]
    public static class StasisRifleFixMod
    {
        private static Dictionary<Creature, float> creatureAggressions = new Dictionary<Creature, float>();

        [HarmonyPatch(nameof(Creature.ScheduledUpdate)), HarmonyPostfix]
        public static void ScheduledUpdate_Postfix(Creature __instance)
        {
            if(!creatureAggressions.ContainsKey(__instance))
            {
                creatureAggressions.Add(__instance, __instance.Aggression.Value);
            }

            __instance.gameObject.TryGetComponent<Rigidbody>(out Rigidbody instanceRB);

            if (instanceRB != null && __instance.isInitialized && StasisRifle.sphere.energy > 0)
            {
                if (StasisRifleFixModOptions.WriteLogs.Value)
                {
                    StasisFreezeFixPlugin.logger.Log(LogLevel.Info, $"Found frozen creature name: {__instance.name}. Current aggression is {creatureAggressions[__instance]}");
                }

                if (instanceRB.isKinematic)
                {
                    __instance.Aggression.Value = 0;
                    __instance.GetAnimator().enabled = false;
                }
                else
                {
                    __instance.Aggression.Value = creatureAggressions[__instance];
                    creatureAggressions.Remove(__instance);
                    __instance.GetAnimator().enabled = true;
                    __instance.UpdateBehaviour(Time.time, Time.deltaTime);
                }
            }

            if (StasisRifleFixModOptions.WriteLogs.Value)
            {
                StasisFreezeFixPlugin.logger.Log(LogLevel.Info, $"Creature's animator.enabled = {__instance.GetAnimator().enabled}");
            }
        }
    }
}
