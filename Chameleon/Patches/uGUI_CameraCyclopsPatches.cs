using Chameleon.Monobehaviors.UI;
using HarmonyLib;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(uGUI_CameraCyclops))]
    internal class uGUI_CameraCyclopsPatches
    {
        private static string[] cameraNames = new string[]
        {
            "ChameleonExternamCam1",
            "ChameleonExternamCam2",
            "ChameleonExternamCam3"
        };

        [HarmonyPatch(nameof(uGUI_CameraCyclops.SetCamera)), HarmonyPostfix]
        private static void SetCamera_Postfix(uGUI_CameraCyclops __instance)
        {
            if(!Player.main.GetCurrentSub().GetComponentInChildren<ChameleonCamoButton>()) return;

            __instance.textTitle.text = string.Empty;

            if(__instance.cameraIndex >= 0 && __instance.cameraIndex < cameraNames.Length)
            {
                __instance.textTitle.text = Language.main.Get(cameraNames[__instance.cameraIndex]);
            }
        }
    }
}
