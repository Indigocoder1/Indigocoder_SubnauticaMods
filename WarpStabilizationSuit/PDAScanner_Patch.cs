﻿using HarmonyLib;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(PDAScanner))]
    internal static class PDAScanner_Patch
    {
        [HarmonyPatch(nameof(PDAScanner.Unlock)), HarmonyPostfix]
        private static void Patch(PDAScanner.EntryData entryData)
        {
            if (entryData.key != TechType.Warper)
            {
                return;
            }

            if (!KnownTech.Contains(Suit_Craftable.itemTechType))
            {
                KnownTech.Add(Suit_Craftable.itemTechType);
            }
        }
    }
}
