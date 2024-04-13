using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    internal static class AtlasHelper
    {
        public static Sprite GetSprite(TechType techType)
        {
            Atlas.Sprite atlasSprite = SpriteManager.Get(techType);

            Rect rect = new Rect(0f, 0f, atlasSprite.texture.width, atlasSprite.texture.height);
            Vector2 vector = new Vector2(atlasSprite.texture.width / 2, atlasSprite.texture.height / 2);
            return Sprite.Create(atlasSprite.texture, rect, vector);
        }
    }
}