using HarmonyLib;
using VariableGravityCannon.Items;
using UnityEngine;
using VariableGravityCannon.Monos;

namespace VariableGravityCannon.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal static class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Update)), HarmonyPostfix]
        private static void Update_Patch()
        {
            InventoryItem heldItem = Inventory.main.quickSlots.heldItem;
            if (heldItem == null) return;

            if (!GameInput.GetButtonDown(GameInput.Button.Deconstruct) || heldItem.techType != VariableGravityCannon_Craftable.techType)
            {
                return;
            }

            VariableGravityCannon_Mono gravCannon = heldItem.item.gameObject.GetComponent<VariableGravityCannon_Mono>();
            gravCannon.ToggleCannonType();

            Inventory.main.quickSlots.SetAnimationState("variablegravitycannon");
        }
    }
}
