using System.Collections.Generic;
using System.Linq;
using TextureReplacerEditor.Monobehaviors.Items;
using TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class MaterialWindow : DraggableWindow
    {
        public Dictionary<OnPropertyChangedEventArgs, object> editChanges { get; private set; } = new();
        public MaterialItem currentMaterialItem { get; private set; }
        public GameObject propertyItemPrefab;
        public Transform propertyItemsParent;

        private Material material;
        private int materialIndex;

        public void SetMaterial(Material material, int index, MaterialItem item)
        {
            this.material = material;
            currentMaterialItem = item;
            materialIndex = index;
            SpawnPropertyItems();
        }

        public void OnPropertyChanged(object sender, OnPropertyChangedEventArgs e)
        {
            e.sender = sender;
            e.materialIndex = materialIndex;
            if (!editChanges.Any(i => i.Key.sender == sender))
            {
                editChanges.Add(e, e.previousValue);
            }
            else
            {
                editChanges[editChanges.First(i => i.Key.sender == sender).Key] = e.previousValue;
            }
        }

        public void OpenConfigWindow()
        {
            TextureReplacerEditorWindow.Instance.configViewerWindow.OpenWindow();
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
    }
}
