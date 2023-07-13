using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using System.Collections.Generic;

namespace StasisRifleFreezeMod_SN
{
    public class StasisFreezeFixMod
    {
        [HarmonyPatch(typeof(Creature))]
        public static class Creature_Patch
        {
            private static Dictionary<Creature, CreatureInfo> creaturesDictionary = new Dictionary<Creature, CreatureInfo>();

            [HarmonyPatch(nameof(Creature.ScheduledUpdate)), HarmonyPostfix]
            public static void ScheduledUpdate_Postfix(Creature __instance)
            {
                if (!creaturesDictionary.ContainsKey(__instance))
                {
                    creaturesDictionary.Add(__instance, new CreatureInfo(__instance.Aggression.Value, false));
                }

                Rigidbody instanceRB = __instance.gameObject.GetComponentInChildren<Rigidbody>();

                if (instanceRB == null)
                {
                    return;
                }

                if (instanceRB != null && StasisRifle.sphere != null)
                {
                    if (QMod.Config.writeLogs && !creaturesDictionary[__instance].wasFrozenLastUpdate)
                    {
                        Logger.Log(Logger.Level.Debug, $"Found frozen creature name: {__instance.name}. Current aggression is {creatureAggressions[__instance]}");
                    }

                    if (instanceRB.isKinematic)
                    {
                        __instance.Aggression.Value = 0;
                        __instance.GetAnimator().enabled = false;

                        if (creaturesDictionary.ContainsKey(__instance))
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

                if (QMod.Config.writeLogs)
                {
                    Logger.Log(Logger.Level.Debug, $"Frozen creature animator.enabled = {__instance.GetAnimator().enabled}");
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
