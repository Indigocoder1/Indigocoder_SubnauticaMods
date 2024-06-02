using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers
{
    internal class TextureChangeHandler : ConfigChangeHandler
    {
        public RawImage originalImage;
        public RawImage newImage;
        public Texture2D nullTextureImage;

        public override void SetInfo(object original, object changed)
        {
            Texture2D oldTex = original as Texture2D;
            Texture2D newTex = changed as Texture2D;

            if (original == null)
            {
                oldTex = nullTextureImage;
            }

            originalImage.texture = oldTex;
            float oldTexRatio = oldTex.width / oldTex.height;
            originalImage.rectTransform.sizeDelta = new Vector2(originalImage.rectTransform.sizeDelta.y * oldTexRatio, originalImage.rectTransform.sizeDelta.y);

            newImage.texture = newTex;
            float newTexRatio = newTex.width / newTex.height;
            newImage.rectTransform.sizeDelta = new Vector2(newImage.rectTransform.sizeDelta.y * newTexRatio, newImage.rectTransform.sizeDelta.y);
        }
    }
}
