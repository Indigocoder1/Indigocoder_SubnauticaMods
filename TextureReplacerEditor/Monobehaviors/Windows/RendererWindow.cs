using TextureReplacerEditor.Monobehaviors.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class RendererWindow : DraggableWindow
    {
        public TextMeshProUGUI rendererNameText;
        public GameObject materialItemPrefab;
        public Transform materialItemsParent;
        private Renderer currentRenderer;

        public void SetRendererInfo(Renderer rend)
        {
            currentRenderer = rend;

            rendererNameText.text = rend.name;
        }

        private void SpawnMaterialItems()
        {
            ClearMaterialItems();

            foreach (var material in currentRenderer.materials)
            {
                MaterialItem materialItem = Instantiate(materialItemPrefab, materialItemsParent).GetComponent<MaterialItem>();
                materialItem.SetInfo(material);
            }
        }

        private void ClearMaterialItems()
        {
            foreach (Transform child in materialItemsParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
