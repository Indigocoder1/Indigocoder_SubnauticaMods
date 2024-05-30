using UnityEngine;
using UnityEngine.EventSystems;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class DraggableWindow : MonoBehaviour, IDragHandler
    {
        public RectTransform root;

        private Canvas canvas;

        private void Start()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            root.anchoredPosition += eventData.delta;
        }
    }
}
