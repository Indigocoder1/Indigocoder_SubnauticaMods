using HarmonyLib;
using IndigocoderLib;
using UnityEngine;
using TMPro;
using Nautilus.Utility;
using System.Collections.Generic;
using Nautilus.Extensions;

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

            Transform abiityTransform = __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities");
            GameObject inputGO = GameObject.Instantiate(Main_Plugin.nameInputFieldGO, abiityTransform);
            inputGO.transform.localPosition = new Vector3(200, 620, 0);

            /*
            Transform cameraButton = abiityTransform.Find("Button_Camera");
            GameObject beaconLaunchGO = GameObject.Instantiate(cameraButton.gameObject, abiityTransform);
            beaconLaunchGO.transform.localPosition = new Vector3(350, 432, 0);
            GameObject.Destroy(beaconLaunchGO.GetComponent<CyclopsExternalCamsButton>());
            CyclopsDecoyLaunchButton decoyButton = abiityTransform.Find("Button_Decoy").GetComponent<CyclopsDecoyLaunchButton>();
            beaconLaunchGO.SetActive(false);
            beaconLaunchGO.AddComponent<CyclopsDecoyLaunchButton>().CopyComponent(decoyButton);
            beaconLaunchGO.SetActive(true);
            */

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
        }
    }
}
