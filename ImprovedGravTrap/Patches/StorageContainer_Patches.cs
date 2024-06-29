using HarmonyLib;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(StorageContainer))]
    internal class StorageContainer_Patches
    {
        [HarmonyPatch(nameof(StorageContainer.Awake)), HarmonyPostfix]
        private static void Awake_Postfix(StorageContainer __instance)
        {
            if (__instance.GetComponentInParent<EnhancedGravSphere>() == null) return;

            __instance.container.isAllowedToAdd = (p, v) =>
            {
                if (p.GetTechType().IsEnhancedGravTrap()) return false;

                return true;
            };
        }
    }
}
