using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(PlayerTool))]
    internal static class PlayerTool_Patch
    {
        /*
        [HarmonyPatch(nameof(PlayerTool.animToolName), MethodType.Getter), HarmonyPostfix]
        private static void AnimToolName_Patch(PlayerTool __instance, ref string __result)
        {
            if (__instance.pickupable?.GetTechType() == TechType.CyclopsDecoy) __result = "beacon";
        }
        */
    }
}
