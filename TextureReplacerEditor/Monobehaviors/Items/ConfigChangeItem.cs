using System.Linq;
using TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers;
using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static TextureReplacerEditor.Monobehaviors.Windows.MaterialWindow;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ConfigChangeItem : MonoBehaviour
    {
        public PropertyEditData propertyEditData { get; private set; }
        public TextMeshProUGUI propertyNameText;
        public ConfigChangeHandler textureChangeHandler;
        public ConfigChangeHandler colorChangeHandler;
        public ConfigChangeHandler floatChangeHandler;
        public ConfigChangeHandler vectorChangeHandler;

        public int EditIndex
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }

        private void Awake()
        {
            textureChangeHandler.gameObject.SetActive(false);
            colorChangeHandler.gameObject.SetActive(false);
            floatChangeHandler.gameObject.SetActive(false);
            vectorChangeHandler.gameObject.SetActive(false);
        }

        public void SetInfo(PropertyEditData propertyEditData)
        {
            this.propertyEditData = propertyEditData;

            propertyNameText.text = propertyEditData.propertyName;
            ConfigChangeHandler handler = propertyEditData.type switch
            {
                ShaderPropertyType.Texture => textureChangeHandler,
                ShaderPropertyType.Color => colorChangeHandler,
                ShaderPropertyType.Float => floatChangeHandler,
                ShaderPropertyType.Range => vectorChangeHandler,
                ShaderPropertyType.Vector => vectorChangeHandler,
                _ => null
            };

            handler.gameObject.SetActive(true);
            handler.SetInfo(propertyEditData.originalValue, propertyEditData.newValue);
        }

        public void RemoveChange()
        {
            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            if(matWindow.materialEdits.Any(i => i.Value.Any(a => a.Equals(propertyEditData))))
            {
                var editDataList = matWindow.materialEdits.FirstOrDefault(i => i.Value.Any(a => a.Equals(propertyEditData)));
                PropertyEditData entry = editDataList.Value.FirstOrDefault(i => i.Equals(propertyEditData));
                editDataList.Value.Remove(entry);
            }

            TextureReplacerEditorWindow.Instance.configViewerWindow.RemoveEdit(this);
            Destroy(gameObject);
        }
    }
}
