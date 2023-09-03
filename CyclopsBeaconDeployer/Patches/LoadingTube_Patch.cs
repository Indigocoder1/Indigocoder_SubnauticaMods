using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyLoadingTube))]
    internal static class LoadingTube_Patch
    {
        [HarmonyPatch(nameof(CyclopsDecoyLoadingTube.Initialize)), HarmonyPostfix]
        private static void Initialize_Patch(CyclopsDecoyLoadingTube __instance)
        {
            //__instance.subRoot.upgradeConsole.modules
        }
    }
}
