using HarmonyLib;

namespace TextureReplacerEditor.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatches
    {
        [HarmonyPatch(nameof(Player.Update)), HarmonyPostfix]
        private static void Update_Postfix(Player __instance)
        {

        }
    }
}
