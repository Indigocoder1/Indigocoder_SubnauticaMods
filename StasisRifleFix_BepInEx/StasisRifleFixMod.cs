using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using System.Collections.Generic;

namespace StasisRifleFixMod_BepInEx
{
    [HarmonyPatch(typeof(Creature))]
    public static class StasisRifleFixMod
    {
        private static Dictionary<Creature, CreatureInfo> creaturesDictionary = new Dictionary<Creature, CreatureInfo>();

        [HarmonyPatch(nameof(Creature.ScheduledUpdate)), HarmonyPostfix]
        public static void ScheduledUpdate_Postfix(Creature __instance)
        {
            if(!creaturesDictionary.ContainsKey(__instance))
            {
                creaturesDictionary.Add(__instance, new CreatureInfo(__instance.Aggression.Value, false));
            }

            Rigidbody instanceRB = __instance.gameObject.GetComponentInChildren<Rigidbody>();

            if(instanceRB == null)
            {
                return;
            }

            if (instanceRB != null && StasisRifle.sphere != null)
            {
                if (StasisFreezeFixPlugin.WriteLogs.Value && !creaturesDictionary[__instance].wasFrozenLastUpdate)
                    StasisFreezeFixPlugin.logger.Log(LogLevel.Info, $"Found frozen creature name: {__instance.name}. Current aggression is {creaturesDictionary[__instance].aggression}");

                if(__instance.GetAnimator() == null)
                {
                    return;
                }

                if (StasisFreezeFixPlugin.WriteLogs.Value && !creaturesDictionary[__instance].wasFrozenLastUpdate)
                    StasisFreezeFixPlugin.logger.Log(LogLevel.Info, $"Creature's animator.enabled = {__instance.GetAnimator().enabled}");

                if (instanceRB.isKinematic)
                {
                    __instance.Aggression.Value = 0;
                    __instance.GetAnimator().enabled = false;

                    if(creaturesDictionary.ContainsKey(__instance))
                    {
                        creaturesDictionary[__instance] = new CreatureInfo(0, true);
                    }
                }
                else
                {
                    __instance.Aggression.Value = creaturesDictionary[__instance].aggression;
                    __instance.GetAnimator().enabled = true;
                    __instance.UpdateBehaviour(Time.time, Time.deltaTime);

                    if (creaturesDictionary.ContainsKey(__instance))
                    {
                        creaturesDictionary.Remove(__instance);
                    }
                }
            }
        }

        private struct CreatureInfo
        {
            public float aggression;
            public bool wasFrozenLastUpdate;

            public CreatureInfo(float aggression, bool wasFrozenLastUpdate)
            {
                this.aggression = aggression;
                this.wasFrozenLastUpdate = wasFrozenLastUpdate;
            }
        }
    }
}
