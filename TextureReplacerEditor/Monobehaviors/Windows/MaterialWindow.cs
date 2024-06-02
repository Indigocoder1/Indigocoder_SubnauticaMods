using System.Collections;
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
        private float searchCallDelay;
        private bool hasSearched;

        public void SetMaterial(Material material, MaterialItem item)
        {
            this.material = material;
            currentMaterialItem = item;
            SpawnPropertyItems();

            searchBar.onValueChanged.AddListener((_) => OnSearchBarValueChanged());
        }

        private void Update()
        {
            if(searchCallDelay > 0)
            {
                searchCallDelay -= Time.unscaledDeltaTime;
            }
            else if(searchCallDelay <= 0 && !hasSearched)
            {
                Search();

                hasSearched = true;
            }
        }

        public void OnPropertyChanged(object sender, OnPropertyChangedEventArgs e)
        {
            if (currentMaterialItem == null) return;

            if (!materialEdits.ContainsKey(currentMaterialItem))
            {
                materialEdits.Add(currentMaterialItem, new List<PropertyEditData>());
            }

            if (e.changedType == ShaderPropertyType.Vector)
            {
                Vector4 nVector = (Vector4)e.newValue;
                Vector4 oVector = (Vector4)e.originalValue;
                if (nVector == oVector) return;
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

        private void OnSearchBarValueChanged()
        {
            Main_Plugin.logger.LogInfo($"Search bar value changed!");
            searchCallDelay = MIN_SEARCH_BAR_WAIT_TIME;
            hasSearched = false;
        }

        private void Search()
        {
            foreach (var propertyItem in propertyItemsParent.GetComponentsInChildren<PropertyItem>(true))
            {
                if (string.IsNullOrEmpty(searchBar.text))
                {
                    propertyItem.gameObject.SetActive(true);
                    continue;
                }

                if (propertyItem.propertyName.ToLower().Contains(searchBar.text.ToLower()))
                {
                    propertyItem.gameObject.SetActive(true);
                }
                else
                {
                    propertyItem.gameObject.SetActive(false);
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
