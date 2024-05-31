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

        public void SetViewingTexture(Texture2D tex)
        {
            textureNameText.text = tex.name;
            textureView.texture = tex;

            float textureRatio = tex.width / tex.height;
            textureView.rectTransform.sizeDelta = new Vector2(targetWidth, targetWidth / textureRatio);
        }
    }
}
