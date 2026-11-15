using HarmonyLib;
using UnityEngine;
using TMPro;
using Nautilus.Utility;
using System.Collections.Generic;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal static class SubRoot_Patch
    {
        public static Dictionary<SubRoot, TMP_InputField> inputFields = new Dictionary<SubRoot, TMP_InputField>();

        [HarmonyPatch(nameof(SubRoot.Start)), HarmonyPostfix]
        private static void Start_Patch(SubRoot __instance)
        {
            if(!__instance.isCyclops)
            {
                return;
            }
            
            if (__instance.name == "__LIGHTMAPPED_PREFAB__") return;

            if (__instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities/BeaconNameInput(Clone)") !=null) return;
            
            Transform abiityTransform = __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities");
            GameObject inputGO = GameObject.Instantiate(Main_Plugin.nameInputFieldGO, abiityTransform);
            Main_Plugin.logger.LogInfo($"Spawning {inputGO} on {__instance}");
            inputGO.transform.localPosition = new Vector3(200, 620, 0);

            TMP_InputField inputField = inputGO.transform.Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>();
            if (!inputFields.ContainsKey(__instance))
            {
                inputFields.Add(__instance, inputField);
            }

            TMP_Text[] text = inputGO.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text item in text)
            {
                item.font = FontUtils.Aller_Rg;
            }

            inputGO.SetActive(false);
        }

        [HarmonyPatch(nameof(SubRoot.OnDestroy)), HarmonyPostfix]
        private static void OnDestroy_Postfix(SubRoot __instance)
        {
            inputFields.Remove(__instance);
        }
    }
}
