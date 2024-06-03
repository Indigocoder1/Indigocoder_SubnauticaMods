using TextureReplacerEditor.Miscellaneous;
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
        private Texture originalTexture;
        private Material material;
        string textureName;

        public override void SetInfo(Material material, string textureName, object overrideValue = null)
        {
            texture = null;
            if (overrideValue == null)
            {
                texture = material.GetTexture(textureName);
            }
            else
            {
                texture = (Texture)overrideValue;
            }

            this.material = material;
            this.textureName = textureName;

            if (texture == null)
            {
                texturePreview.texture = nullTexImage;
                return;
            }

            UpdateTexturePreview();
        }

        public void SetAsViewingTexture()
        {
            if (texture is Texture3D)
            {
                ErrorMessage.AddError("Previewing 3D textures is not supported!");
                return;
            }

            if (texture == null) return;

            TextureReplacerEditorWindow.Instance.textureViewWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.textureViewWindow.SetViewingTexture(texture as Texture2D);
        }

        public void LoadTextureFromDisk()
        {
            Texture2D tex = TextureLoadSaveHandler.LoadTexture();
            if (tex == null) return;

            texture = tex;
            material.SetTexture(textureName, texture);

            InvokeOnPropertyChanged(new(originalTexture, tex, UnityEngine.Rendering.ShaderPropertyType.Texture));
        }

        public void SaveTextureToDisk()
        {
            if (texture == null)
            {
                ErrorMessage.AddMessage("<color=#FCFF00>Texture is null. Cannot save!</color>");
                return;
            }

            TextureLoadSaveHandler.SaveTexture(texture);
        }

        public override void UpdateMaterial()
        {
            Texture texture = material.GetTexture(textureName);
            if(texture == this.texture) return;

            UpdateTexturePreview();
        }

        private void UpdateTexturePreview()
        {
            texturePreview.texture = texture;
            float texRatio = texture.width / texture.height;
            texturePreview.rectTransform.sizeDelta = new Vector2(targetPreviewScale * texRatio, targetPreviewScale);
        }
    }
}
