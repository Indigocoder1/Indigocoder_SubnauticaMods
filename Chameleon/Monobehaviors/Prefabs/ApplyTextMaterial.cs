using UnityEngine;
using TMPro;
using Nautilus.Utility;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class ApplyTextMaterial : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().font = FontUtils.Aller_Rg;
        }
    }
}
