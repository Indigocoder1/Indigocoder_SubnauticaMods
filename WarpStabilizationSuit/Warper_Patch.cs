using HarmonyLib;
using Nautilus.Handlers;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(Warper))]
    internal static class Warper_Patch
    {
        [HarmonyPatch(nameof(Warper.OnKill)), HarmonyPostfix]
        private static void Patch()
        {
            if (!KnownTech.Contains(Suit_Craftable.suitTechType))
            {
                KnownTech.Add(Suit_Craftable.suitTechType);

                PDAEncyclopedia.Add("WarpStabilizationSuit", true);
            }
        }
    }
}
