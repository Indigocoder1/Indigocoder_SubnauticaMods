using HarmonyLib;

namespace TextureReplacer.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal static class Player_Patches
    {
        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Patch(Player __instance)
        {
            if(CustomTextureReplacer.queuedPlayerConfigs.Count <= 0)
            {
                return;
            }

            __instance.gameObject.SetActive(false);
            TextureReplacerHelper textureReplacerHelper = __instance.gameObject.EnsureComponent<TextureReplacerHelper>();
            for (int i = CustomTextureReplacer.queuedPlayerConfigs.Count - 1; i >= 0; i--)
            {
                textureReplacerHelper.AddTextureData(CustomTextureReplacer.queuedPlayerConfigs[i]);
                CustomTextureReplacer.queuedPlayerConfigs.RemoveAt(i);
            }

            __instance.gameObject.SetActive(true);
        }
    }
}
