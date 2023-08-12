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
            public static string GetActionString()
            {
                return Main_Plugin.AdvanceKey.Value.ToString();
            }

            public static int GetChangeListDir()
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
                    return Input.GetKeyDown(Main_Plugin.AdvanceKey.Value) ? 1 : 0;
                }
            }
        }

        private static bool IsGravTrap(this TechType techType) => techType == TechType.Gravsphere || techType == Trap_Craftable.techType;

        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
        static class TooltipFactory_ItemCommons_Patch
        {
            static void Postfix(StringBuilder sb, TechType techType, GameObject obj)
            {
                if (!techType.IsGravTrap())
                    return;

                var objectsType = obj.EnsureComponent<GravTrapObjectsType>();
                objectsType.techTypeListIndex += TypeListSwitcher.GetChangeListDir();
                TooltipFactory.WriteDescription(sb, $"Allowrd type = {objectsType.GetCurrentListName()}");
            }
        }

        [HarmonyPatch(typeof(TooltipFactory), "ItemActions")]
        static class TooltipFactory_ItemActions_Patch
        {
            static readonly string button = TypeListSwitcher.GetActionString();

            [HarmonyPostfix]
            static void Postfix(StringBuilder sb, InventoryItem item)
            {
                if (item.techType.IsGravTrap())
                {
                    TooltipFactory.WriteAction(sb, button, "Switch object's type");
                } 
            }
        }

        private static class ExtraGUITextPatch
        {
            [HarmonyPostfix, HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
            static void GUIHand_OnUpdate_Postfix(GUIHand __instance)
            {
                if (!__instance.player.IsFreeToInteract() || !AvatarInputHandler.main.IsEnabled())
                    return;

                if (__instance.GetTool() is PlayerTool tool && tool.pickupable?.GetTechType().IsGravTrap() == true)
                {
                    string text = tool.GetCustomUseText();
                    text += $"\n{tool.gameObject.GetComponent<GravTrapObjectsType>().GetCurrentListName()}";
                    HandReticle.main.SetText(HandReticle.TextType.Use, text, true);
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnHandHover))]
            static void Pickupable_OnHandHover_Postfix(Pickupable __instance)
            {
                if (__instance.GetTechType().IsGravTrap())
                    HandReticle.main.SetText(HandReticle.TextType.Use, __instance.gameObject.GetComponent<GravTrapObjectsType>().GetCurrentListName(), true);
            }
        }
    }
}
