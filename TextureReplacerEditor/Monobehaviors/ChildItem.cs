using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class ChildItem : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public RectTransform childIndexRect;

        public void SetChildInfo(string name, int childIndex)
        {
            nameText.text = name;

            Vector2 sizeDelta = childIndexRect.sizeDelta;
            sizeDelta.x = 10 * childIndex;
            childIndexRect.sizeDelta = sizeDelta;
        }
    }
}
