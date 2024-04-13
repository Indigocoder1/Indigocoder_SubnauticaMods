using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    internal class CustomItem : MonoBehaviour
    {
        public TextMeshProUGUI nameText;

        public void SetItemName(string name)
        {
            nameText.text = name;
        }

        public void SetActiveItem()
        {

        }
    }
}
