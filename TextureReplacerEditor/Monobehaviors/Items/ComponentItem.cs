using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ComponentItem : MonoBehaviour
    {
        public Component component {get; private set;}
        public TextMeshProUGUI text;

        public void SetInfo(Component component)
        {
            this.component = component;
            text.text = component.GetType().ToString();
        }

        public void TryOpenMaterialWindow()
        {
            if(component is not Renderer)
            {
                return;
            }

            TextureReplacerEditorWindow.Instance.rendererWindow.OpenWindow();
            TextureReplacerEditorWindow.Instance.rendererWindow.SetRendererInfo(component as Renderer);
        }
    }
}
