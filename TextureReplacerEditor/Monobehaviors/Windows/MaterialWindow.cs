using TextureReplacerEditor.Monobehaviors.Items;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class MaterialWindow : DraggableWindow
    {
        public GameObject propertyItemPrefab;
        public Transform propertyItemsParent;
        private Material material;

        public void SetMaterial(Material material)
        {
            this.material = material;
            SpawnPropertyItems();
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
