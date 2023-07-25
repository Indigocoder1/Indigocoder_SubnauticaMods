using HarmonyLib;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(Warper))]
    internal static class Warper_Patch
    {
        [HarmonyPatch(nameof(Warper.OnKill)), HarmonyPostfix]
        private static void Patch()
        {
            if (!KnownTech.Contains(Suit_Craftable.itemTechType))
            {
                KnownTech.Add(Suit_Craftable.itemTechType);
            }
        }
    }
}
