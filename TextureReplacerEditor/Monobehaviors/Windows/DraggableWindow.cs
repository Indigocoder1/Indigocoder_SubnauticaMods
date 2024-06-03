using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class DraggableWindow : MonoBehaviour
    {
        private bool initialized;

        public void OpenWindow()
        {
            gameObject.SetActive(true);
            if(!initialized)
            {
                gameObject.SetActive(false);
                gameObject.SetActive(true);
                initialized = true;
            }

            TextureReplacerEditorWindow.Instance.SetWindowActive(this, true);
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);
            TextureReplacerEditorWindow.Instance.SetWindowActive(this, false);
        }
    }
}
