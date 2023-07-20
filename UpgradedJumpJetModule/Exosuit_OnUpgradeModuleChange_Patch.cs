using HarmonyLib;
using System.Threading.Tasks;

namespace UpgradedJumpJetModule
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_OnUpgradeModuleChange_Patch
    {
        [HarmonyPatch(nameof(Exosuit.OnUpgradeModuleChange)), HarmonyPostfix]
        private static void Patch(Exosuit __instance, TechType techType)
        {
            bool currentlyHasModule = __instance.modules.GetCount(UpgradedJetsModule.moduleTechType) > 0;
            Main_Plugin.logger.LogInfo($"Jump jets upgraded = {__instance.jumpJetsUpgraded}");
            if (techType == UpgradedJetsModule.moduleTechType && currentlyHasModule)
            {
                __instance.jumpJetsUpgraded = true;
            }
        }
    }
}
