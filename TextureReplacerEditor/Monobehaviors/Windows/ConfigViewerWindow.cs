using System.Collections.Generic;
using TextureReplacerEditor.Monobehaviors.Items;
using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static TextureReplacer.CustomTextureReplacer;

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
        private List<CustomConfigItem> addedItems;
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

            List<ConfigInfo.TextureEdit> textureEdits = new();
            foreach (var item in window.editChanges)
            {
                textureEdits.Add(GetTextureEdit(item.Key));
            }

            ConfigInfo info = new($"MyCoolConfig{createdItems}", window.currentMaterialItem.prefabIdentifierRoot.ClassId, window.currentMaterialItem.pathToRenderer,
                false, 0, new List<string>(), textureEdits);
            configItem.SetInfo(info);

            addedItems.Add(configItem);

            createdItems++;

            SetCurrentConfig(currentItem);
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
            foreach (var item in configsParent)
            {
                
            }
        }

        private ConfigInfo.TextureEdit GetTextureEdit(OnPropertyChangedEventArgs args)
        {
            PropertyItem propertyItem = args.sender as PropertyItem;
            TextureEditType type = propertyItem.propertyType switch
            {
                ShaderPropertyType.Color => TextureEditType.Color,
                ShaderPropertyType.Vector => TextureEditType.Vector,
                ShaderPropertyType.Float => TextureEditType.Float,
                ShaderPropertyType.Range => TextureEditType.Float,
                ShaderPropertyType.Texture => TextureEditType.Texture,
                _ => TextureEditType.Texture
            };

            string data = args.changedType switch
            {
                ShaderPropertyType.Color => FormatColor(args.newValue),
                ShaderPropertyType.Vector => FormatVector(args.newValue),
                ShaderPropertyType.Float => ((float)args.newValue).ToString(),
                ShaderPropertyType.Range => ((float)args.previousValue).ToString(),
                ShaderPropertyType.Texture => ((Texture2D)args.newValue).name,
                _ => "INVALID INPUT"
            };

            return new(args.materialIndex, type, propertyItem.propertyName, data);
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
