using Nautilus.Utility;
using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    internal class ApplySNFont : MonoBehaviour
    {
        public FontApplyMode mode;

        private void Start ()
        {
            switch (mode)
            {
                case FontApplyMode.GameObject:
                    GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
                    break;
                case FontApplyMode.Children:
                    foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
                    {
                        text.font = FontUtils.Aller_Rg;
                    }
                    break;
                case FontApplyMode.GameObjectAndChildren:
                    GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
                    foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
                    {
                        text.font = FontUtils.Aller_Rg;
                    }
                    break;
            }
            
        }
    }

    public enum FontApplyMode
    {
        GameObject,
        Children,
        GameObjectAndChildren
    }
}
