using UnityEngine;
using UnityEngine.EventSystems;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class DraggableWindowHandle : MonoBehaviour, IDragHandler
    {
        public RectTransform rect;
        private Canvas canvas;

        private void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            rect = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            rect.anchoredPosition += eventData.delta;
        }
    }
}
