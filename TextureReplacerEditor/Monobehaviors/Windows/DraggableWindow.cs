using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class DraggableWindow : MonoBehaviour
    {
        public void OpenWindow()
        {
            gameObject.SetActive(true);
            TextureReplacerEditorWindow.Instance.SetWindowActive(this, true);
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
            TextureReplacerEditorWindow.Instance.SetWindowActive(this, false);
        }
    }
}
