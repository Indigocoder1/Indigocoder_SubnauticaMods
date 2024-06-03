using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ChildItem : MonoBehaviour
    {
        [SerializeField] public GameObject originalChild { get; private set; }
        [SerializeField] public string pathToChild { get; private set; }
        public TextMeshProUGUI nameText;
        public RectTransform childIndexRect;

        public void SetChildInfo(string name, int childIndex, GameObject originalChild, string pathToChild)
        {
            Main_Plugin.logger.LogInfo($"Setting path to child to {pathToChild} on {gameObject}");

            nameText.text = name;
            this.originalChild = originalChild;

            Vector2 sizeDelta = childIndexRect.sizeDelta;
            sizeDelta.x = 10 * childIndex;
            childIndexRect.sizeDelta = sizeDelta;
        }

        public void SetCurrentSelectedItem()
        {
            TextureReplacerEditorWindow.Instance.prefabInfoWindow.SetCurrentItem(this);
        }
    }
}
