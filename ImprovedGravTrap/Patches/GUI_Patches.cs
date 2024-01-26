using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using UnityEngine;
using System.Text;

namespace ImprovedGravTrap.Patches
{
    internal static class GUI_Patches
    {
        private static class TypeListSwitcher
        {
            public static string GetAdvanceKey()
            {
                return "F";//GameInput.Button.AltTool.ToString();
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
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return GameInput.GetButtonDown(GameInput.Button.AltTool) ? 1 : 0;
                }
            }
        }

        #region ---TooltipFactory ---
        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
        [HarmonyPostfix]
        private static void ItemCommons_Postfix(StringBuilder sb, TechType techType, GameObject obj)
        {
            if (!techType.IsEnhancedGravTrap())
                return;

            Main_Plugin.logger.LogInfo("Patching item commons for an enhanced grav trap");

            var objectsType = obj.EnsureComponent<GravTrapObjectsType>();
            objectsType.techTypeListIndex += TypeListSwitcher.GetListChangeDelta();
            TooltipFactory.WriteDescription(sb, $"Allowed type = {objectsType.GetCurrentListName()}");
        } 

        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemActions))]
        [HarmonyPostfix]
        private static void ItemActions_Postfix(StringBuilder sb, InventoryItem item) 
        {
            Main_Plugin.logger.LogInfo($"Item {item.techType} is enhanced grav trap = {item.techType.IsEnhancedGravTrap()}");
            if (!item.techType.IsEnhancedGravTrap())
            {
                return;
            }

            string button = TypeListSwitcher.GetAdvanceKey();
            StorageContainer storageContainer = item.item.gameObject.GetComponentInChildren<StorageContainer>(true);

            Main_Plugin.logger.LogInfo("Patching item actions for an enhanced grav trap");
            TooltipFactory.WriteAction(sb, button, "Switch object's type"); //Advance type display in inventory

            /*
            if (!storageContainer) return;
            
            // Open/close storage prompt:
            string msg = storageContainer.GetOpen() ? "Close Storage" : "Open Storage";
            bool usingController = GameInput.GetPrimaryDevice() == GameInput.Device.Controller;
            
            TooltipFactory.WriteAction(sb,
                uGUI.FormatButton(usingController ? GameInput.Button.Sprint : GameInput.Button.AltTool), msg);
            */
        }
        #endregion

        [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
        [HarmonyPostfix]
        static void GUIHand_OnUpdate_Postfix(GUIHand __instance)
        {
            if (!__instance.player.IsFreeToInteract() || !AvatarInputHandler.main.IsEnabled())
                return;

            if (__instance.GetTool() is PlayerTool tool && tool.pickupable?.GetTechType().IsEnhancedGravTrap() == true)
            {
                string text = tool.GetCustomUseText();
                text += $"\n{tool.gameObject.GetComponent<GravTrapObjectsType>().GetCurrentListName()}";
                HandReticle.main.SetText(HandReticle.TextType.Use, text, true);
            }
        }

        [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnHandHover))]
        [HarmonyPostfix]
        static void Pickupable_OnHandHover_Postfix(Pickupable __instance)
        {
            if (__instance.GetTechType().IsEnhancedGravTrap())
                HandReticle.main.SetText(HandReticle.TextType.Use, __instance.gameObject.GetComponent<GravTrapObjectsType>().GetCurrentListName(), true);
        }
    }
}
