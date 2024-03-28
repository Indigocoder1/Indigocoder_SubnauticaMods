using Chameleon.Interfaces;
using UnityEngine;
using TMPro;

namespace Chameleon.Monobehaviors.UI
{
    internal class DepthUIManager : MonoBehaviour, IUIElement
    {
        public CrushDamage crushDamage;
        public TextMeshProUGUI depthText;

        public void UpdateUI()
        {
            int currentDepth = (int)crushDamage.GetDepth();
            int maxDepth = (int)crushDamage.crushDepth;
            Color textColor = currentDepth > maxDepth ? Color.red : Color.white;

            depthText.text = $"{currentDepth}m / {maxDepth}m";
            depthText.color = textColor;
        }
    }
}
