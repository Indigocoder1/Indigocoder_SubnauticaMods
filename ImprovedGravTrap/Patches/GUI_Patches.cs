using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using UnityEngine;
using System.Text;
using static VFXParticlesPool;

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
                return GameInput.GetBinding(device, GameInput.Button.Deconstruct, GameInput.BindingSet.Primary);
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

                return GameInput.GetButtonDown(GameInput.Button.Deconstruct) ? 1 : 0;
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

            //Storage opening
            StorageContainer container = obj.GetComponentInChildren<StorageContainer>();
            
            if (!IngameMenu.main.selected && GameInput.GetButtonDown(GameInput.Button.AltTool))
            {
                PDA pda = Player.main.GetPDA();

                if(!container.GetOpen())
                {
                    //Remove player equipment UI
                    pda.ui.OnClosePDA();

                    //Open storage container
                    container.Open(obj.transform);

                    //Have to set open manually since PDA.Open returns false (it's already open) and it won't be set in StorageContainer.Open
                    container.open = true;

                    //Refresh UI to show container contents
                    pda.ui.OnOpenPDA(PDATab.Inventory);
                    pda.ui.Select();
                    pda.ui.OnPDAOpened();
                    pda.onCloseCallback = pda =>
                    {
                        container.OnClosePDA(pda);
                    };
                }
                else
                {
                    //Remove storage UI
                    pda.ui.OnClosePDA();

                    //Clear used storage
                    Inventory.main.ClearUsedStorage();
                    container.OnClosePDA(Player.main.pda);

                    //Refresh UI
                    pda.ui.OnOpenPDA(PDATab.Inventory);
                    pda.ui.Select();
                    pda.ui.OnPDAOpened();
                }
            }
        } 

        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemActions))]
        [HarmonyPostfix]
        private static void ItemActions_Postfix(StringBuilder sb, InventoryItem item) 
        {
            if (!item.techType.IsEnhancedGravTrap())
                return;

            string button = TypeListSwitcher.GetAdvanceKey();
            TooltipFactory.WriteAction(sb, button, "Switch object's type"); //Display advance type prompt in inventory

            GameInput.Device device = GameInput.GetPrimaryDevice();
            string altToolKey = GameInput.GetBinding(device, GameInput.Button.AltTool, GameInput.BindingSet.Primary);

            StorageContainer container = item.item.GetComponentInChildren<StorageContainer>();

            TooltipFactory.WriteAction(sb, altToolKey, container.open ? "Close Storage" : "Open Storage");
        }
        #endregion

        [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnHandHover))]
        [HarmonyPostfix]
        static void Pickupable_OnHandHover_Postfix(Pickupable __instance)
        {
            if (!__instance.GetTechType().IsEnhancedGravTrap())
                return;

            string currentMode = __instance.gameObject.GetComponent<GravTrapObjectsType>().GetCurrentListName();
            HandReticle.main.SetText(HandReticle.TextType.Use, $"Advance mode ({currentMode})", false, GameInput.Button.Deconstruct);
        }
    }
}
