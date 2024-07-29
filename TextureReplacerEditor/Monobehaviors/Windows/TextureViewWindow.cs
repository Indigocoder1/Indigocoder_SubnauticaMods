using TextureReplacerEditor.Miscellaneous;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class TextureViewWindow : DraggableWindow
    {
        public TextMeshProUGUI textureNameText;
        public RawImage textureView;
        public float targetWidth;
        private Texture2D currentTexture;

        public void SetViewingTexture(Texture2D tex)
        {
            currentTexture = tex;

            textureNameText.text = tex.name;
            textureView.texture = tex;

            float textureRatio = (float)tex.width / tex.height;
            textureView.rectTransform.sizeDelta = new Vector2(targetWidth, targetWidth / textureRatio);
        }

        public void SaveCurrentTexture()
        {
            TextureLoadSaveHandler.SaveTexture(currentTexture);
        }
    }
}
