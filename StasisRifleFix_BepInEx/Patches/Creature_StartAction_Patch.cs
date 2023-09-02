using HarmonyLib;
using System;

namespace StasisRifleFixMod_BepInEx
{
    [HarmonyPatch(typeof(Creature))]
    internal static class Creature_StartAction_Patch
    {
        [HarmonyPatch(nameof(Creature.TryStartAction)), HarmonyPrefix]
        private static bool Patch(CreatureAction action)
        {
            if(Main_Plugin.WriteLogs.Value)
            {
                bool isFrozen = action.creature.GetComponent<StasisRifleFix_Component>().IsFrozen();
                Main_Plugin.logger.LogInfo($"Creature {action.creature.name}.IsFrozen() = {isFrozen}");
            }
                

            if(action.creature.GetComponent<StasisRifleFix_Component>().IsFrozen())
            {
                return false;
            }

            return true;
        }
    }
}
