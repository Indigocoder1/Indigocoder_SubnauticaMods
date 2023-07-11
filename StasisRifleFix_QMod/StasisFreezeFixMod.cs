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
            private static Dictionary<Creature, float> creatureAggressions = new Dictionary<Creature, float>();

            [HarmonyPatch(nameof(Creature.ScheduledUpdate)), HarmonyPostfix]
            public static void ScheduledUpdate_Postfix(Creature __instance)
            {
                Creature instance = __instance;

                creatureAggressions.Add(instance, instance.Aggression.Value);
                Rigidbody instanceRB = instance.gameObject.GetComponent<Rigidbody>();

                if (instanceRB != null && instance.isInitialized)
                {
                    if (QMod.Config.writeLogs)
                    {
                        Logger.Log(Logger.Level.Debug, $"Found frozen creature name: {instance.name}. Current aggression is {creatureAggressions[instance]}");
                    }

                    if (instanceRB.isKinematic)
                    {
                        instance.Aggression.Value = 0;
                        instance.GetAnimator().enabled = false;
                    }
                    else
                    {
                        instance.Aggression.Value = creatureAggressions[instance];
                        creatureAggressions.Remove(instance);
                        instance.GetAnimator().enabled = true;
                        instance.UpdateBehaviour(Time.deltaTime);
                    }
                }

                if (QMod.Config.writeLogs)
                {
                    Logger.Log(Logger.Level.Debug, $"Frozen creature animator.enabled = {instance.GetAnimator().enabled}");
                }
            }
        }
    }
}
