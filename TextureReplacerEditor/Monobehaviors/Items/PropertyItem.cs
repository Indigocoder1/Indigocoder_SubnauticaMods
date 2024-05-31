using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
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

        private void Awake()
        {
            textureModeHandler.gameObject.SetActive(false);
            colorModeHander.gameObject.SetActive(false);
            floatModeHandler.gameObject.SetActive(false);
            vectorModeHandler.gameObject.SetActive(false);
        }

        public void SetInfo(ShaderPropertyType propertyType, Material material, string propertyName)
        {
            propertyNameText.text = propertyName;
            propertyTypeText.text = propertyType.ToString();

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
        }
    }
}
