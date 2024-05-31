using TextureReplacerEditor.Monobehaviors.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class TextureModeHandler : PropertyHandler
    {
        public RawImage texturePreview;
        public Texture2D nullTexImage;
        public float targetPreviewScale;
        private Texture texture;

        public override void SetInfo(Material material, string textureName)
        {
            texture = material.GetTexture(textureName);
            if(texture == null)
            {
                texturePreview.texture = nullTexImage;
                return;
            }

            texturePreview.texture = texture;
            float texRatio = texture.width / texture.height;
            texturePreview.rectTransform.sizeDelta = new Vector2(targetPreviewScale * texRatio, targetPreviewScale);
        }

        public void SetAsViewingTexture()
        {
            if(texture is Texture3D)
            {
                ErrorMessage.AddError("Previewing 3D textures is not supported!");
                return;
            }

            if (texture == null) return;

            TextureReplacerEditorWindow.Instance.textureViewWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.textureViewWindow.SetViewingTexture(texture as Texture2D);
        }
    }
}
