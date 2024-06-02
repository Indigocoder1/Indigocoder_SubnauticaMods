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

        public void SetInfo(ShaderPropertyType propertyType, Material material, string propertyName)
        {
            propertyNameText.text = propertyName;
            propertyTypeText.text = propertyType.ToString();

            this.propertyName = propertyName;
            this.propertyType = propertyType;
            this.material = material;

            switch (propertyType)
            {
                case ShaderPropertyType.Texture:
                    textureModeHandler.gameObject.SetActive(true);
                    textureModeHandler.SetInfo(material, propertyName);
                    break;
                case ShaderPropertyType.Color:
                    colorModeHander.gameObject.SetActive(true);
                    colorModeHander.SetInfo(material, propertyName);
                    break;
                case ShaderPropertyType.Float:
                    floatModeHandler.gameObject.SetActive(true);
                    floatModeHandler.SetInfo(material, propertyName);
                    break;
                case ShaderPropertyType.Range:
                    floatModeHandler.gameObject.SetActive(true);
                    floatModeHandler.SetInfo(material, propertyName);
                    break;
                case ShaderPropertyType.Vector:
                    vectorModeHandler.gameObject.SetActive(true);
                    vectorModeHandler.SetInfo(material, propertyName);
                    break;
            }

            if(propertyName == "_Color" && propertyType == ShaderPropertyType.Color)
            {
                isMainColor = true;
            }
        }

        private void OnDestroy()
        {
            PropertyHandler.OnPropertyChanged -= TextureReplacerEditorWindow.Instance.materialWindow.OnPropertyChanged;
        }
    }
}
