using System.Collections.Generic;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class TextureReplacerEditorWindow : MonoBehaviour
    {
        public static TextureReplacerEditorWindow Instance;
        public static List<DraggableWindow> activeWindows;

        public bool WindowActive
        {
            get
            {
                return activeWindows.Count > 0;
            }
        }

        public PrefabInfoWindow prefabInfoWindow;
        public RendererWindow rendererWindow;
        private uGUI_InputGroup inputGroup;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            inputGroup = GetComponent<uGUI_InputGroup>();
        }

        public void SetWindowActive(bool active)
        {
            Time.timeScale = active ? 0 : 1;
            gameObject.SetActive(active);
        }
    }
}
