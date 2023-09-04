using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal static class Player_Patch
    {
        [HarmonyPatch(nameof(Player.CanEject)), HarmonyPostfix]
        private static void CanEject(Player __instance, ref bool __result)
        {
            if (__instance.currentMountedVehicle != null)
            {
                return;
            }

            if(__instance.currentSub == null)
            {
                return;
            }

            __result = !SubRoot_Patch.inputFields[__instance.currentSub].isFocused;
        }
    }
}
