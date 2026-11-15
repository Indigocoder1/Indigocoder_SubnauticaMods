using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace IndigocoderLib
{
    public static class ImageHelper
    {
        public static Sprite GetSpriteFromAssetsFolder(string imageName, string asetFolderPath = null)
        {
            string spriteFilePath = "";
            if (!string.IsNullOrEmpty(asetFolderPath))
            {
                spriteFilePath = Path.Combine(asetFolderPath + $"/{imageName}");
            }
            else
            {
                spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + $"/{imageName}";
            }
            return ImageUtils.LoadSpriteFromFile(spriteFilePath);
        }
    }
}