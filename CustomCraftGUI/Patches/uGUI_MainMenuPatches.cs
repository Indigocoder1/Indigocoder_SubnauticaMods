using CustomCraftGUI.Monobehaviors;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace CustomCraftGUI.Patches
{
    [HarmonyPatch(typeof(uGUI_MainMenu))]
    internal class uGUI_MainMenuPatches
    {
        [HarmonyPatch(nameof(uGUI_MainMenu.Start)), HarmonyPostfix]
        private static void Start_Postfix(uGUI_MainMenu __instance)
        {
            GameObject customCraftGUICanvas = GameObject.Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("CustomCraftGUICanvas"));
            customCraftGUICanvas.SetActive(false);
            Canvas canvas = customCraftGUICanvas.GetComponent<Canvas>(); 
            canvas.sortingLayerName = "XMenu";
            canvas.planeDistance = 1;

            customCraftGUICanvas.GetComponentInChildren<ItemIconSpawner>(true).SpawnIcons();

            Transform menuButtons = __instance.transform.Find("Panel/MainMenu/PrimaryOptions/MenuButtons");

            GameObject ccButton = GameObject.Instantiate(menuButtons.Find("ButtonPlay").gameObject, menuButtons);
            ccButton.transform.SetSiblingIndex(1);
            ccButton.name = "ButtonCustomCraftGUI";

            Transform text = ccButton.transform.Find("Circle/Bar/Text");
            uGUI_GraphicRaycaster menuRaycaster = __instance.GetComponent<uGUI_GraphicRaycaster>();

            GameObject.Destroy(text.GetComponent<TranslationLiveUpdate>());
            text.GetComponent<TextMeshProUGUI>().text = "Custom Craft Editor";

            Button ccMenuButton = ccButton.GetComponent<Button>();
            ccMenuButton.onClick.RemoveAllListeners();
            ccMenuButton.onClick.AddListener(() => 
            { 
                customCraftGUICanvas.SetActive(true);
                menuRaycaster.enabled = false;
            });

            Button closeButton = customCraftGUICanvas.transform.Find("VersionSelectionMenu/CloseButton").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() =>
            {
                customCraftGUICanvas.SetActive(false);
                menuRaycaster.enabled = true;
            });
        }
    }
}
