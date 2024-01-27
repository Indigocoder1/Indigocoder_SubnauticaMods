using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using UnityEngine;
using System.Text;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch]
    internal static class GUI_Patches
    {
        private static class TypeListSwitcher
        {
            public static string GetAdvanceKey()
            {
                GameInput.Device device = GameInput.GetPrimaryDevice();
                return GameInput.GetBinding(device, GameInput.Button.AltTool, GameInput.BindingSet.Primary);
            }

            public static int GetListChangeDelta()
            {
                if (Main_Plugin.UseScrollWheel.Value)
                {
                    if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    {
                        return 1;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    {
                        return -1;
                    }
                }

                return GameInput.GetButtonDown(GameInput.Button.AltTool) ? 1 : 0;
            }
        }

        #region ---TooltipFactory ---
        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
        [HarmonyPostfix]
        private static void ItemCommons_Postfix(StringBuilder sb, TechType techType, GameObject obj)
        {
            if (!techType.IsEnhancedGravTrap())
                return;

            GravTrapObjectsType objectsType = obj.EnsureComponent<GravTrapObjectsType>();
            int changeDelta = TypeListSwitcher.GetListChangeDelta();

            //Handle index overflow
            int nextIndex = (objectsType.techTypeListIndex + changeDelta) % Main_Plugin.AllowedTypes.Count;

            //Handle negative scoll wheel input wrap around
            objectsType.techTypeListIndex = (objectsType.techTypeListIndex == 0 && changeDelta < 0) ? Main_Plugin.AllowedTypes.Count - 1 : nextIndex;
            TooltipFactory.WriteDescription(sb, $"Allowed type = {objectsType.GetCurrentListName()}");
        } 

        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemActions))]
        [HarmonyPostfix]
        private static void ItemActions_Postfix(StringBuilder sb, InventoryItem item) 
        {
            if (!item.techType.IsEnhancedGravTrap())
                return;

            string button = TypeListSwitcher.GetAdvanceKey();
            TooltipFactory.WriteAction(sb, button, "Switch object's type"); //Display advance type prompt in inventory
        }
        #endregion

        [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnHandHover))]
        [HarmonyPostfix]
        static void Pickupable_OnHandHover_Postfix(Pickupable __instance)
        {
            if (!__instance.GetTechType().IsEnhancedGravTrap())
                return;

            HandReticle.main.SetText(HandReticle.TextType.Use, __instance.gameObject.GetComponent<GravTrapObjectsType>().GetCurrentListName(), true);
        }
    }
}
