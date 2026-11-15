using HarmonyLib;

namespace TextureReplacer.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class CyclopsPatches
    {
        [HarmonyPatch(nameof(SubRoot.Start)), HarmonyPostfix]
        private static void Start_Postfix(SubRoot __instance)
        {
            if (!__instance.isCyclops) return;

            if (CustomTextureReplacer.queuedCyclopsConfigs.Count <= 0)
            {
                return;
            }

            __instance.gameObject.SetActive(false);
            var textureReplacerHelper = __instance.gameObject.EnsureComponent<TextureReplacerHelper>();

            for (int i = CustomTextureReplacer.queuedCyclopsConfigs.Count - 1; i >= 0; i--)
            {
                textureReplacerHelper.AddTextureData(CustomTextureReplacer.queuedCyclopsConfigs[i]);
                CustomTextureReplacer.queuedCyclopsConfigs.RemoveAt(i);
            }

            __instance.gameObject.SetActive(true);
        }
    }
}
