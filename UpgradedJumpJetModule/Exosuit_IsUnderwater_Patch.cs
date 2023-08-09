using HarmonyLib;

namespace UpgradedJumpJetModule
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_IsUnderwater_Patch
    {
        [HarmonyPatch(nameof(Exosuit.IsUnderwater)), HarmonyPostfix]
        private static void Patch(Exosuit __instance, ref bool __result)
        {
            if (__instance.modules.GetCount(UpgradedJetsModule.techType) > 0)
            {
                __result = true;
            }
        }
    }
}
