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

        public Material material { get; private set; }
        public TextMeshProUGUI materialNameText;
        public RawImage texturePreview;
        public Texture2D nullTextureImage;
        public ActiveColorPreview activeColorPreview;
        public GameObject tutorialHighlight;
        public Transform tutorialLineTarget;

        private Texture2D mainTex;

        public void SetInfo(Material material, PrefabIdentifier prefabIdentifier, string pathToRenderer)
        {
            this.material = material;
            this.pathToRenderer = pathToRenderer;
            prefabIdentifierRoot = prefabIdentifier;
            materialNameText.text = material.name.Remove(material.name.Length - 9) + $"{transform.GetSiblingIndex()})";

            activeColorPreview.OnColorChanged += () =>
            {
                material.color = activeColorPreview.GetCurrentColor();
            };

            UpdatePreviews();
        }

        private void Update()
        {
            if(material.color != activeColorPreview.GetCurrentColor())
            {
                activeColorPreview.SetActiveColor(material.color);
            }
        }

        private void UpdatePreviews()
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

            TextureReplacerEditorWindow.Instance.tutorialHandler.TriggerTutorialItem("OnMaterialWindowOpened");
        }

        public void ViewMaterialMainTex()
        {
            TextureReplacerEditorWindow.Instance.textureViewWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.textureViewWindow.SetViewingTexture(mainTex);
        }

        public void SetHighlightActive(bool active)
        {
            tutorialHighlight.SetActive(active);
        }
    }
}
