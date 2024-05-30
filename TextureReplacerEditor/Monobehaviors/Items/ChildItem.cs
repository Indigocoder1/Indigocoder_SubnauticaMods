using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ChildItem : MonoBehaviour
    {
        public GameObject originalChild { get; private set; }
        public TextMeshProUGUI nameText;
        public RectTransform childIndexRect;
        private PrefabInfoWindow infoWindow;

        public void SetChildInfo(string name, int childIndex, GameObject originalChild, PrefabInfoWindow infoWindow)
        {
            nameText.text = name;
            this.infoWindow = infoWindow;
            this.originalChild = originalChild;

            Vector2 sizeDelta = childIndexRect.sizeDelta;
            sizeDelta.x = 10 * childIndex;
            childIndexRect.sizeDelta = sizeDelta;
        }

        public void SetCurrentSelectedItem()
        {
            infoWindow.SetCurrentItem(this);
        }
    }
}
