using HarmonyLib;
using IndigocoderLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(Player))]
    internal static class EncySetup
    {
        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Patch()
        {
            string bannerFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/warpStabilizationEncyBanner.png";
            Texture2D bannerImage = ImageUtils.LoadTextureFromFile(bannerFilePath);

            string popupFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/warpStabilizationPopup.png";
            Sprite popupImage = SpriteHelper.SpriteFromAtlasSprite(ImageUtils.LoadSpriteFromFile(popupFilePath));

            LanguageHandler.SetLanguageLine("Tech/Equipment", "Warp Stabilization Suit");

            PDAHandler.AddEncyclopediaEntry("WarpStabilizationSuit", "Tech/Equipment", "Warp Stabilization Suit", 
                "Using technology scanned from alien \"Warpers\" this suit has been designed to prevent teleportation from unknown sources. \n" +
                "\n" +
                "-While wearing this suit warpers' teleportation technology will not be effective. \n" +
                "-It will not be possible to be warped out of vehicles. \n" +
                "-Warpers will most likely use other methods to attack.",
                bannerImage,
                popupImage,
                PDAHandler.UnlockImportant
                );

            PDAHandler.AddCustomScannerEntry(TechType.Warper, 6f, false, "WarpStabilizationSuit");
        }
    }
}
