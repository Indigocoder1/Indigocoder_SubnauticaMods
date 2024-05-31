using System.Collections.Generic;
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
        }

        public void OnDrag(PointerEventData eventData)
        {
            rect.anchoredPosition += eventData.delta;
        }
    }
}
