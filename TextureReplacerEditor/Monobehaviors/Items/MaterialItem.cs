using System;
using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class MaterialItem : MonoBehaviour
    {
        public PrefabIdentifier prefabIdentifierRoot { get; private set; }
        public string pathToRenderer { get; private set; }

        public int MaterialIndex
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }

        public TextMeshProUGUI materialNameText;
        public RawImage texturePreview;
        public Texture2D nullTextureImage;
        public ActiveColorPreview activeColorPreview;

        private Material material;
        private Texture2D mainTex;

        public void SetInfo(Material material, PrefabIdentifier prefabIdentifier, string pathToRenderer)
        {
            this.material = material;
            this.pathToRenderer = pathToRenderer;
            prefabIdentifierRoot = prefabIdentifier;
            materialNameText.text = material.name;

            activeColorPreview.OnColorChanged += () =>
            {
                material.color = activeColorPreview.GetCurrentColor();
            };

            UpdatePreviews(null, null);
            PropertyHandler.OnPropertyChanged += UpdatePreviews;
        }

        private void UpdatePreviews(object sender, OnPropertyChangedEventArgs e)
        {
            activeColorPreview.SetActiveColor(material.color);

            if (material.HasProperty("_MainTex"))
            {
                mainTex = material.GetTexture("_MainTex") as Texture2D;
                texturePreview.texture = mainTex;
            }
            else
            {
                texturePreview.texture = nullTextureImage;
            }
        }

        public void OpenMaterialWindow()
        {
            TextureReplacerEditorWindow.Instance.materialWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.materialWindow.SetMaterial(material, this);
        }

        public void ViewMaterialMainTex()
        {
            TextureReplacerEditorWindow.Instance.textureViewWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.textureViewWindow.SetViewingTexture(mainTex);
        }
    }
}
