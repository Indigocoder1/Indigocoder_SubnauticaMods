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
            var component = action.creature.GetComponent<StasisRifleFix_Component>();
            if (!component) return false;

            if (Main_Plugin.WriteLogs.Value)
            {
                bool isFrozen = component.IsFrozen();
                Main_Plugin.logger.LogInfo($"Creature {action.creature.name}.IsFrozen() = {isFrozen}");
            }
                

            if(component.IsFrozen())
            {
                return false;
            }

            return true;
        }
    }
}
