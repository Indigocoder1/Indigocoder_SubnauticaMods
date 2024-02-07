using HarmonyLib;
using VariableGravityCannon.Items;
using VariableGravityCannon.Monos;

namespace VariableGravityCannon.Patches
{
    [HarmonyPatch(typeof(Inventory))]
    internal static class InventoryPatches
    {
        [HarmonyPatch(nameof(Inventory.GetHeldTool)), HarmonyPostfix]
        private static void GetHeldTool_Postfix(Inventory __instance, ref PlayerTool __result)
        {
            if(__instance.GetHeld() == null)
            {
                return;
            }

            if(__instance.GetHeld().GetTechType() != VariableGravityCannon_Craftable.techType)
            {
                return;
            }

            VariableGravityCannon_Mono variableGravCannonMono = __instance.GetHeld().GetComponent<VariableGravityCannon_Mono>();
            __result = variableGravCannonMono.GetActiveCannon();
        }
    }
}
