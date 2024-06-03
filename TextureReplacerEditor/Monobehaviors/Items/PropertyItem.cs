using System;
using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class PropertyItem : MonoBehaviour
    {
        public TextMeshProUGUI propertyNameText;
        public TextMeshProUGUI propertyTypeText;
        public PropertyHandler textureModeHandler;
        public PropertyHandler colorModeHander;
        public PropertyHandler floatModeHandler;
        public PropertyHandler vectorModeHandler;

        public string propertyName { get; private set; }
        public ShaderPropertyType propertyType { get; private set; }
        public Material material { get; private set; }

        private bool isMainColor;
        private PropertyHandler activeHandler;

        private void Awake()
        {
            textureModeHandler.gameObject.SetActive(false);
            colorModeHander.gameObject.SetActive(false);
            floatModeHandler.gameObject.SetActive(false);
            vectorModeHandler.gameObject.SetActive(false);
        }

        private void Start()
        {
            PropertyHandler.OnPropertyChanged += TextureReplacerEditorWindow.Instance.materialWindow.OnPropertyChanged;
            TextureReplacerEditorWindow.Instance.configViewerWindow.OnChangeDeleted += UpdateHandlerValues;
        }

        private void Update()
        {
            if (!isMainColor)
            {
                return;
            }

            if (material.color != (colorModeHander as ColorModeHandler).activeColorPreview.GetCurrentColor())
            {
                (colorModeHander as ColorModeHandler).UpdateColor();
            }
        }

        public void SetInfo(ShaderPropertyType propertyType, Material material, string propertyName, object overrideOriginal = null)
        {
            propertyNameText.text = propertyName;
            propertyTypeText.text = propertyType.ToString();

            this.propertyName = propertyName;
            this.propertyType = propertyType;
            this.material = material;

            activeHandler = propertyType switch
            {
                ShaderPropertyType.Texture => textureModeHandler,
                ShaderPropertyType.Color => colorModeHander,
                ShaderPropertyType.Range => floatModeHandler,
                ShaderPropertyType.Float => floatModeHandler,
                ShaderPropertyType.Vector => vectorModeHandler,
                _ => null
            };

            activeHandler.gameObject.SetActive(true);
            activeHandler.SetInfo(material, propertyName, overrideOriginal);

            if(propertyName == "_Color" && propertyType == ShaderPropertyType.Color)
            {
                isMainColor = true;
            }
        }

        private void OnDestroy()
        {
            PropertyHandler.OnPropertyChanged -= TextureReplacerEditorWindow.Instance.materialWindow.OnPropertyChanged;
            TextureReplacerEditorWindow.Instance.configViewerWindow.OnChangeDeleted -= UpdateHandlerValues;
        }

        private void UpdateHandlerValues(object sender, ConfigViewerWindow.OnChangeDeleteArgs e)
        {
            if (activeHandler.propertyName != e.propertyName) return;

            activeHandler.UpdateMaterial();
        }
    }
}
