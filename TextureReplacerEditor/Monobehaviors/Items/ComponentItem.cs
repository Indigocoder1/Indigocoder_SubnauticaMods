using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ComponentItem : MonoBehaviour
    {
        public Component component { get; private set; }
        public TextMeshProUGUI text;
        private string pathToComponent;

        public void SetInfo(Component component, string pathToComponent)
        {
            this.component = component;
            this.pathToComponent = pathToComponent;
            text.text = component.GetType().ToString();
        }

        public void TryOpenMaterialWindow()
        {
            if (component is not Renderer)
            {
                return;
            }

            TextureReplacerEditorWindow.Instance.rendererWindow.OpenWindow();

            PrefabIdentifier prefabIdentifier = TextureReplacerEditorWindow.Instance.prefabInfoWindow.currentPrefabIdentifier;
            TextureReplacerEditorWindow.Instance.rendererWindow.SetRendererInfo(component as Renderer, prefabIdentifier, pathToComponent);
        }
    }
}
