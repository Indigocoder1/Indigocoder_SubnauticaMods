using System.Collections.Generic;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class TextureReplacerEditorWindow : MonoBehaviour
    {
        public static TextureReplacerEditorWindow Instance;
        public static List<DraggableWindow> activeWindows { get; private set; } = new();

        public bool IsWindowActive
        {
            get
            {
                return activeWindows.Count > 0;
            }
        }

        public PrefabInfoWindow prefabInfoWindow;
        public RendererWindow rendererWindow;
        public MaterialWindow materialWindow;
        public TextureViewWindow textureViewWindow;
        private uGUI_InputGroup inputGroup;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            inputGroup = GetComponent<uGUI_InputGroup>();
        }

        public void SetWindowActive(DraggableWindow window, bool active)
        {
            if(active && !activeWindows.Contains(window))
            {
                activeWindows.Add(window);
            }
            else
            {
                activeWindows.Remove(window);
            }

            Time.timeScale = activeWindows.Count > 0 ? 0 : 1;
            gameObject.SetActive(activeWindows.Count > 0);
        }
    }
}
