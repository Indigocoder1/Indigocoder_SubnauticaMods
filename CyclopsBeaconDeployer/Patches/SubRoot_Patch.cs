using HarmonyLib;
using IndigocoderLib;
using UnityEngine;
using TMPro;
using Nautilus.Utility;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal static class SubRoot_Patch
    {
        [HarmonyPatch(nameof(SubRoot.Start)), HarmonyPostfix]
        private static void Start_Patch(SubRoot __instance)
        {
            if(Utilities.GetNameWithCloneRemoved(__instance.name) == "Cyclops-MainPrefab")
            {
                Transform abiityTransform = __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_RightHUD/Abilities");
                GameObject go = GameObject.Instantiate(Main_Plugin.nameInputFieldGO, abiityTransform);
                go.transform.localPosition = new Vector3(200, 620, 0);

                TMP_Text[] text = go.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text item in text)
                {
                    item.font = FontUtils.Aller_Rg;
                }
            }
        }
    }
}
