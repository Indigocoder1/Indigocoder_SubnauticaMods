using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(Pickupable))]
    internal class Pickupable_Patches
    {
        [HarmonyPatch(nameof(Pickupable.OnHandHover)), HarmonyPostfix]
        private static void OnHandHover_Postfix(Pickupable __instance)
        {
            if (!__instance.TryGetComponent(out EnhancedGravSphere _)) return;

            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Open storage", false, GameInput.Button.AltTool);
        }
    }
}
