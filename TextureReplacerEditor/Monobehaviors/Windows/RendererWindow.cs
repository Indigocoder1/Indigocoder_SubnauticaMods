using TextureReplacerEditor.Monobehaviors.Items;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class RendererWindow : DraggableWindow
    {
        public TextMeshProUGUI rendererNameText;
        public GameObject materialItemPrefab;
        public Transform materialItemsParent;

        private Renderer currentRenderer;
        private PrefabIdentifier prefabIdentifier;
        private string pathToRend;

        public void SetRendererInfo(Renderer rend, PrefabIdentifier prefabIdentifier, string pathToRend)
        {
            currentRenderer = rend;
            this.pathToRend = pathToRend;
            this.prefabIdentifier = prefabIdentifier;

            rendererNameText.text = rend.name;

            SpawnMaterialItems();
        }

        private void SpawnMaterialItems()
        {
            ClearMaterialItems();

            foreach (var material in currentRenderer.materials)
            {
                MaterialItem materialItem = Instantiate(materialItemPrefab, materialItemsParent).GetComponent<MaterialItem>();
                materialItem.SetInfo(material, prefabIdentifier, pathToRend);
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
