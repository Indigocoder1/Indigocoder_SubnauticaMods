using TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ConfigChangeItem : MonoBehaviour
    {
        public TextMeshProUGUI propertyNameText;
        public ConfigChangeHandler textureChangeHandler;
        public ConfigChangeHandler colorChangeHandler;
        public ConfigChangeHandler floatChangeHandler;
        public ConfigChangeHandler vectorChangeHandler;

        private void Awake()
        {
            textureChangeHandler.gameObject.SetActive(false);
            colorChangeHandler.gameObject.SetActive(false);
            floatChangeHandler.gameObject.SetActive(false);
            vectorChangeHandler.gameObject.SetActive(false);
        }

        public void SetInfo(string propertyName, object originalValue, object newValue, ShaderPropertyType type)
        {
            propertyNameText.text = propertyName;
            ConfigChangeHandler handler = type switch
            {
                ShaderPropertyType.Texture => textureChangeHandler,
                ShaderPropertyType.Color => colorChangeHandler,
                ShaderPropertyType.Float => floatChangeHandler,
                ShaderPropertyType.Range => vectorChangeHandler,
                ShaderPropertyType.Vector => vectorChangeHandler,
                _ => null
            };

            handler.gameObject.SetActive(true);
            handler.SetInfo(originalValue, newValue);
        }

        public void RemoveChange()
        {

        }
    }
}
