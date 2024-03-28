using Chameleon.Interfaces;
using TMPro;
using UnityEngine;

namespace Chameleon.Monobehaviors.UI
{
    internal class PowerUIManager : MonoBehaviour, IUIElement
    {
        public SubRoot subRoot;
        public TextMeshProUGUI powerText;

        public void UpdateUI()
        {
            float normalizedPower = subRoot.powerRelay.GetMaxPower() / subRoot.powerRelay.GetMaxPower();
            int currentPower = subRoot.powerRelay.GetMaxPower() == 0f ? 0 : Mathf.CeilToInt(normalizedPower * 100f);

            powerText.text = $"{currentPower}%";
        }
    }
}
