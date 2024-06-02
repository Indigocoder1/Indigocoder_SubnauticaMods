using System.Collections.Generic;
using TextureReplacerEditor.Monobehaviors.Items;
using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static TextureReplacer.CustomTextureReplacer;
using static TextureReplacerEditor.Monobehaviors.Windows.MaterialWindow;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class ConfigViewerWindow : DraggableWindow
    {
        public TMP_InputField configNameTextField;
        public GameObject configItemPrefab;
        public Transform configsParent;
        public GameObject configChangePrefab;
        public Transform configChangesParent;

        private CustomConfigItem currentItem;
        private List<CustomConfigItem> addedItems = new();
        private int createdItems;

        private void Start()
        {
            configNameTextField.onValueChanged.AddListener((_) =>
            {
                if (currentItem == null) return;

                currentItem.configInfo.configName = configNameTextField.text;
            });
        }

        public void SetCurrentConfig(CustomConfigItem item)
        {
            currentItem = item;
            configNameTextField.text = currentItem.configInfo.configName;

            ClearEditedItems();
        }

        public void AddConfigItem()
        {
            CustomConfigItem configItem = Instantiate(configItemPrefab, configsParent).GetComponent<CustomConfigItem>();
            currentItem = configItem;

            MaterialWindow window = TextureReplacerEditorWindow.Instance.materialWindow;

            Main_Plugin.logger.LogInfo($"Prefab identifier = {window.currentMaterialItem.prefabIdentifierRoot}");

            ConfigInfo info = new($"MyCoolConfig{createdItems}", window.currentMaterialItem.prefabIdentifierRoot.ClassId, window.currentMaterialItem.pathToRenderer,
                false, 0, new List<string>(), GetCurrentWindowTextureEdits());

            configItem.SetInfo(info);
            addedItems.Add(configItem);

            createdItems++;

            SetCurrentConfig(currentItem);
            SpawnEditedItems();
        }

        private void ClearEditedItems()
        {
            foreach (Transform child in configChangesParent)
            {
                Destroy(child);
            }
        }

        private void SpawnEditedItems()
        {
            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            foreach (var propertyData in matWindow.materialEdits[matWindow.currentMaterialItem])
            {
                ConfigChangeItem changeItem = Instantiate(configChangePrefab, configChangesParent).GetComponent<ConfigChangeItem>();
                changeItem.SetInfo(propertyData.propertyName, propertyData.originalValue, propertyData.newValue, propertyData.type);
            }
        }

        private List<ConfigInfo.TextureEdit> GetCurrentWindowTextureEdits()
        {
            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            List<PropertyEditData> propertyData = matWindow.materialEdits[matWindow.currentMaterialItem];
            List<ConfigInfo.TextureEdit> edits = new();
            foreach (var data in propertyData)
            {
                edits.Add(GetTextureEdit(data));
            }

            return edits;
        }

        private ConfigInfo.TextureEdit GetTextureEdit(PropertyEditData editData)
        {
            TextureEditType type = editData.type switch
            {
                ShaderPropertyType.Color => TextureEditType.Color,
                ShaderPropertyType.Vector => TextureEditType.Vector,
                ShaderPropertyType.Float => TextureEditType.Float,
                ShaderPropertyType.Range => TextureEditType.Float,
                ShaderPropertyType.Texture => TextureEditType.Texture,
                _ => TextureEditType.Texture
            };

            string data = editData.type switch
            {
                ShaderPropertyType.Color => FormatColor(editData.newValue),
                ShaderPropertyType.Vector => FormatVector(editData.newValue),
                ShaderPropertyType.Float => ((float)editData.newValue).ToString(),
                ShaderPropertyType.Range => ((float)editData.newValue).ToString(),
                ShaderPropertyType.Texture => ((Texture2D)editData.newValue).name,
                _ => "INVALID INPUT"
            };

            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            return new(matWindow.currentMaterialItem.MaterialIndex, type, editData.propertyName, data);
        }

        private string FormatColor(object val)
        {
            Color col = (Color)val;
            return $"{col.r},{col.g},{col.b},{col.a}";
        }

        private string FormatVector(object val)
        {
            Vector4 vector = (Vector4)val;
            return $"{vector.x},{vector.y},{vector.z},{vector.w}";
        }
    }
}
