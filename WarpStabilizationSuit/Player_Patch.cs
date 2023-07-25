using HarmonyLib;
using IndigocoderLib.SpriteHelper;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(Player))]
    internal static class Player_Patch
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
                "Warpers' observation scanners have informed them that while you are wearing the suit, teleportation will not be effective. \n" +
                "Warpers instead have been observed coming in closer for slashing blows which deal large amounts of damage.",
                bannerImage,
                popupImage,
                PDAHandler.UnlockImportant
                );

            PDAHandler.AddCustomScannerEntry(TechType.Warper, 6f, false, "WarpStabilizationSuit");
        }
    }
}
