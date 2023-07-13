using HarmonyLib;
using BepInEx.Logging;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_OnModuleChanged_Patch
    {
        [HarmonyPatch(nameof(Exosuit.OnUpgradeModuleChange)), HarmonyPrefix]
        private static bool Patch(Exosuit __instance, int slotID, TechType techType)
        {
            if (techType != GrapplingArmUpgradeModule.TechType)
            {
                return true;
            }

            __instance.MarkArmsDirty();
            return false;
        }
    }
}
