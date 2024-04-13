using UnityEngine;

namespace CustomCraftGUI.Utilities
{
    internal static class SpriteSizeFormatter
    {
        public static Vector2 GetSpriteShrinkScalar(Atlas.Sprite sprite)
        {
            Vector2 textureSize = new Vector2(sprite.texture.width, sprite.texture.height);
            float sizeRatio = textureSize.x / textureSize.y;

            if (textureSize != new Vector2(128, 128) && textureSize.sqrMagnitude < 4000000 && sizeRatio != 0.5f)
            {
                Vector2 sizeScalar = new Vector2(128, 128) / textureSize;

                return sizeScalar;
            }

            return Vector2.one;
        }
    }
}
