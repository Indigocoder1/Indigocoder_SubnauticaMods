using HarmonyLib;
using IndigocoderLib;

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

            if(!__instance.currentSub.isCyclops)
            {
                return;
            }

            if(SubRoot_Patch.inputFields.TryGetValue(__instance.currentSub, out var inputField))
            {
                __result = !inputField.isFocused;
            }
            else
            {
                Main_Plugin.logger.LogError($"Input field not found for sub: {__instance.currentSub.name}!");
            }
        }
    }
}
