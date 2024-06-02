using IndigocoderLib;
using System.Collections.Generic;
using System.Drawing.Imaging;
using TextureReplacerEditor.Monobehaviors.Items;
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
                currentItem.configNameText.text = configNameTextField.text;
            });
        }

        public void SetCurrentConfig(CustomConfigItem item)
        {
            currentItem = item;
            configNameTextField.text = currentItem.configInfo.configName;

            SpawnEditedItems();
        }

        public void DeleteConfig(CustomConfigItem item)
        {
            addedItems.Remove(item);
            ClearEditedItems();
        }

        public void AddConfigItem()
        {
            CustomConfigItem configItem = Instantiate(configItemPrefab, configsParent).GetComponent<CustomConfigItem>();
            currentItem = configItem;

            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;

            ConfigInfo info = new($"MyCoolConfig{createdItems}", matWindow.currentMaterialItem.prefabIdentifierRoot.ClassId, matWindow.currentMaterialItem.pathToRenderer,
                false, 0, new List<string>(), GetCurrentWindowTextureEdits());

            string prefabName = Utilities.GetNameWithCloneRemoved(matWindow.currentMaterialItem.prefabIdentifierRoot.name);
            configItem.SetInfo(info, matWindow.materialEdits[matWindow.currentMaterialItem], prefabName);
            addedItems.Add(configItem);

            createdItems++;

            SetCurrentConfig(currentItem);
            SpawnEditedItems();
        }

        private void ClearEditedItems()
        {
            foreach (Transform child in configChangesParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void SpawnEditedItems()
        {
            ClearEditedItems();

            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            foreach (var propertyData in currentItem.propertyEdits)
            {
                ConfigChangeItem changeItem = Instantiate(configChangePrefab, configChangesParent).GetComponent<ConfigChangeItem>();
                changeItem.SetInfo(propertyData.propertyName, propertyData.originalValue, propertyData.newValue, propertyData.type);
            }
        }

        private List<ConfigInfo.TextureEdit> GetCurrentWindowTextureEdits()
        {
            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            if (!matWindow.materialEdits.ContainsKey(matWindow.currentMaterialItem)) return new();

            List<PropertyEditData> propertyData = matWindow.materialEdits[matWindow.currentMaterialItem];
            List<ConfigInfo.TextureEdit> edits = new();
            foreach (var data in propertyData)
            {
                edits.Add(GetTextureEdit(data));
            }

            return edits;
        }
        
        public void RemoveEdit(int editIndex)
        {
            currentItem.configInfo.textureEdits.RemoveAt(editIndex);

            PropertyEditData editData = currentItem.propertyEdits[editIndex];
            switch (editData.type)
            {
                case ShaderPropertyType.Texture:
                    editData.propertyItem.material.SetTexture(editData.propertyName, editData.originalValue as Texture);
                    break;
                case ShaderPropertyType.Color:
                    editData.propertyItem.material.SetColor(editData.propertyName, (Color)editData.originalValue);
                    break;
                case ShaderPropertyType.Float:
                    editData.propertyItem.material.SetFloat(editData.propertyName, (float)editData.originalValue);
                    break;
                case ShaderPropertyType.Range:
                    editData.propertyItem.material.SetFloat(editData.propertyName, (float)editData.originalValue);
                    break;
                case ShaderPropertyType.Vector:
                    editData.propertyItem.material.SetVector(editData.propertyName, (Vector4)editData.originalValue);
                    break;
            }
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
