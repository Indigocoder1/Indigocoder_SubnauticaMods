using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(uGUI_Equipment))]
    internal class uGUI_EquipmentPatches
    {
        [HarmonyPatch(nameof(uGUI_Equipment.Awake)), HarmonyPrefix]
        private static void AwakePrefix(uGUI_Equipment __instance)
        {
            uGUI_EquipmentSlot slot = CloneSlot(__instance, "SeamothModule1", "ChameleonUpgrade1");
            ApplyChangesToChameleonModuleImage(slot.transform.Find("Seamoth").GetComponent<Image>());
            CloneSlot(__instance, "SeamothModule2", "ChameleonUpgrade2");
            CloneSlot(__instance, "SeamothModule3", "ChameleonUpgrade3");
            CloneSlot(__instance, "SeamothModule4", "ChameleonUpgrade4");
        }

        private static void ApplyChangesToChameleonModuleImage(Image image)
        {
            image.sprite = Main_Plugin.AssetBundle.LoadAsset<Sprite>("ChameleonModulesBackground");
            image.name = "Chameleon";
            image.transform.localScale = new Vector3(1f, 2.7f, 1f);
        }

        private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName)
        {
            Transform newSlot = GameObject.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
            newSlot.name = newSlotName;
            uGUI_EquipmentSlot equipmentSlot = newSlot.GetComponent<uGUI_EquipmentSlot>();
            equipmentSlot.slot = newSlotName;
            return equipmentSlot;
        }
    }
}
