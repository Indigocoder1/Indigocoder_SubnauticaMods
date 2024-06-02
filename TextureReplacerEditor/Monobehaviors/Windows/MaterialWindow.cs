using System.Collections.Generic;
using System.Linq;
using TextureReplacerEditor.Monobehaviors.Items;
using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class MaterialWindow : DraggableWindow
    {
        private const float MIN_SEARCH_BAR_WAIT_TIME = 0.5f;

        public MaterialItem currentMaterialItem { get; private set; }
        public Dictionary<MaterialItem, List<PropertyEditData>> materialEdits { get; private set; } = new();

        public TMP_InputField searchBar;
        public GameObject propertyItemPrefab;
        public Transform propertyItemsParent;

        private Material material;

        public void SetMaterial(Material material, MaterialItem item)
        {
            this.material = material;
            currentMaterialItem = item;
            SpawnPropertyItems();

            searchBar.onValueChanged.AddListener((_) => OnSearchBarValueChange());
        }

        public void OnPropertyChanged(object sender, OnPropertyChangedEventArgs e)
        {
            if (currentMaterialItem == null) return;

            Main_Plugin.logger.LogInfo($"Sender = {sender} | New value = {e.newValue} | Original value = {e.originalValue}");
            if (e.newValue == e.originalValue) return;

            if(!materialEdits.ContainsKey(currentMaterialItem))
            {
                materialEdits.Add(currentMaterialItem, new List<PropertyEditData>());
            }

            PropertyItem propertyItem = (sender as MonoBehaviour).GetComponentInParent<PropertyItem>();
            if (materialEdits[currentMaterialItem].Any(i => i.propertyItem == propertyItem))
            {
                PropertyEditData item = materialEdits[currentMaterialItem].First(i => i.propertyItem == propertyItem);
                int index = materialEdits[currentMaterialItem].IndexOf(item);
                materialEdits[currentMaterialItem][index] = new PropertyEditData(propertyItem, e.originalValue, e.newValue, propertyItem.propertyName, e.changedType);
            }
            else
            {
                materialEdits[currentMaterialItem].Add(new PropertyEditData(propertyItem, e.originalValue, e.newValue, propertyItem.propertyName, e.changedType));
            }
        }

        public void OpenConfigWindow()
        {
            TextureReplacerEditorWindow.Instance.configViewerWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.configViewerWindow.AddConfigItem();
        }

        private void ClearPropertyItems()
        {
            foreach (Transform child in propertyItemsParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void SpawnPropertyItems()
        {
            ClearPropertyItems();

            int propertyCount = material.shader.GetPropertyCount();
            for (int i = 0; i < propertyCount; i++)
            {
                ShaderPropertyType type = material.shader.GetPropertyType(i);
                string propertyName = material.shader.GetPropertyName(i);
                PropertyItem propertyItem = Instantiate(propertyItemPrefab, propertyItemsParent).GetComponent<PropertyItem>();
                propertyItem.SetInfo(type, material, propertyName);
            }
        }

        private void OnSearchBarValueChange()
        {
            Main_Plugin.logger.LogInfo($"Search bar value changed!");
            CancelInvoke(nameof(SearchItems));
            Invoke(nameof(SearchItems), MIN_SEARCH_BAR_WAIT_TIME);
        }

        private void SearchItems()
        {
            foreach (var propertyItem in propertyItemsParent.GetComponentsInChildren<PropertyItem>())
            {
                if(string.IsNullOrEmpty(searchBar.text))
                {
                    propertyItem.gameObject.SetActive(true);
                    continue;
                }

                if(propertyItem.propertyName.Contains(searchBar.text))
                {
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public struct PropertyEditData
        {
            public PropertyItem propertyItem;
            public object originalValue;
            public object newValue;
            public string propertyName;
            public ShaderPropertyType type;

            public PropertyEditData(PropertyItem propertyItem, object originalValue, object newValue, string propertyName, ShaderPropertyType type)
            {
                this.propertyItem = propertyItem;
                this.originalValue = originalValue;
                this.newValue = newValue;
                this.propertyName = propertyName;
                this.type = type;
            }
        }
    }
}
