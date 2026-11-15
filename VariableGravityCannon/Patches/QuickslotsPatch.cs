using HarmonyLib;
using ImprovedGravTrap.Items;
using ImprovedGravTrap.Monos;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(QuickSlots))]
    internal static class QuickslotsPatch
    {
        [HarmonyPatch(nameof(QuickSlots.SetAnimationState)), HarmonyPrefix]
        private static bool SetAnimationState_Postfix(QuickSlots __instance, string toolName)
        {
            Main_Plugin.logger.LogInfo(toolName);

            if (toolName != VariableGravityCannon_Craftable.techType.ToString().ToLower())
                return true;

            string name = __instance.heldItem.item.GetComponent<VariableGravityCannon_Mono>().IsPropActive() ? "propulsioncannon" : "repulsioncannon";
            __instance.SetAnimationState(name);
            return false;
        }
    }
}
