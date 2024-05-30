using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class DraggableWindow : MonoBehaviour
    {
        public void OpenWindow()
        {
            gameObject.SetActive(true);
            TextureReplacerEditorWindow.activeWindows.Add(this);
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
            TextureReplacerEditorWindow.activeWindows.Remove(this);
        }
    }
}
