using IndigocoderLib;
using System;
using System.Collections.Generic;
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
        public event EventHandler<OnChangeDeleteArgs> OnChangeDeleted;
        public List<CustomConfigItem> addedItems { get; private set; } = new();

        public TMP_InputField configNameTextField;
        public GameObject configItemPrefab;
        public Transform configsParent;
        public GameObject configChangePrefab;
        public Transform configChangesParent;

        private CustomConfigItem currentItem;
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
            InfoMessageWindow messageWindow = TextureReplacerEditorWindow.Instance.messageWindow;
            messageWindow.OpenWindow();
            messageWindow.OpenPrompt($"Are you sure you want to delete {item.configInfo.configName}?", Color.red, "Yes", "No", () =>
            {
                addedItems.Remove(item);
                createdItems--;
                ClearEditedItems();
                Destroy(item.gameObject);

                if (addedItems.Count > 0)
                {
                    SetCurrentConfig(addedItems[addedItems.Count - 1]);
                }
            }, null);
        }

        public void AddConfigItem()
        {
            CustomConfigItem configItem = Instantiate(configItemPrefab, configsParent).GetComponent<CustomConfigItem>();
            currentItem = configItem;

            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;

            ConfigInfo info = new($"MyCoolConfig{createdItems}", matWindow.currentMaterialItem.prefabIdentifierRoot.ClassId, matWindow.currentMaterialItem.pathToRenderer,
                false, 0, new List<string>(), GetTextureEdits(matWindow.CurrentMaterialEditData));

            string prefabName = Utilities.GetNameWithCloneRemoved(matWindow.currentMaterialItem.prefabIdentifierRoot.name);
            bool currentWindowHasEdits = matWindow.materialEdits.ContainsKey(matWindow.CurrentMaterialEditData);
            List<PropertyEditData> edits = currentWindowHasEdits ? matWindow.materialEdits[matWindow.CurrentMaterialEditData] : new();

            configItem.SetInfo(info, edits, prefabName, matWindow.CurrentMaterialEditData);
            addedItems.Add(configItem);

            createdItems++;

            SetCurrentConfig(currentItem);
            SpawnEditedItems();
        }

        public void UpdateConfigItem(CustomConfigItem item)
        {
            item.configInfo.textureEdits = GetTextureEdits(item.materialEditData);
            SetCurrentConfig(item);
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
                changeItem.SetInfo(propertyData);
            }
        }

        private List<ConfigInfo.TextureEdit> GetTextureEdits(MaterialEditData editMaterialData)
        {
            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            if (!matWindow.materialEdits.ContainsKey(editMaterialData)) return new();

            List<PropertyEditData> propertyData = matWindow.materialEdits[editMaterialData];
            List<ConfigInfo.TextureEdit> edits = new();
            foreach (var data in propertyData)
            {
                edits.Add(GetTextureEdit(data, editMaterialData));
            }

            return edits;
        }
        
        public void RemoveEdit(ConfigChangeItem item)
        {
            currentItem.configInfo.textureEdits.RemoveAt(item.EditIndex);
            currentItem.propertyEdits.Remove(item.propertyEditData);

            PropertyEditData editData = item.propertyEditData;
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

            OnChangeDeleted?.Invoke(this, new(editData.propertyName));
        }

        private ConfigInfo.TextureEdit GetTextureEdit(PropertyEditData editData, MaterialEditData materialData)
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

            return new(materialData.materialIndex, type, editData.propertyName, data);
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

        public class OnChangeDeleteArgs : EventArgs
        {
            public string propertyName;

            public OnChangeDeleteArgs(string propertyName)
            {
                this.propertyName = propertyName;
            }
        }
    }
}
