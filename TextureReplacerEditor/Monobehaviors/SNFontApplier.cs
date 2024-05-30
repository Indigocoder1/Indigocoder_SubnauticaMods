using Nautilus.Utility;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class SNFontApplier : MonoBehaviour
    {
        private void Start()
        {
            foreach (var item in GetComponentsInChildren<TextMeshProUGUI>())
            {
                item.font = FontUtils.Aller_Rg;
            }
        }
    }
}
