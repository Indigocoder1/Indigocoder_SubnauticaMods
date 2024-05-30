using HarmonyLib;
using TextureReplacerEditor.Monobehaviors;
using UnityEngine;

namespace TextureReplacerEditor.Patches
{
    [HarmonyPatch(typeof(UWE.Utils))]
    internal class UtilsPatches
    {
        [HarmonyPatch(nameof(UWE.Utils.UpdateCusorLockState)), HarmonyPrefix]
        private static bool UpdateCursorLockState_Prefix()
        {
            if(TextureReplacerEditorWindow.windowActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return false;
            }

            return true;
        }
    }
}
