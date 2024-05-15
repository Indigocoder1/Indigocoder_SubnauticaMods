using Chameleon.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Chameleon.Monobehaviors.UI
{
    internal class NoiseUIManager : MonoBehaviour, IUIElement
    {
        public CyclopsNoiseManager noiseManager;
        public Image noiseBar;

        public void UpdateUI()
        {
            float noisePercent = noiseManager.GetNoisePercent();
            noiseBar.fillAmount = Mathf.Lerp(noiseBar.fillAmount, noisePercent, Time.deltaTime);
        }

        public void OnSubDestroyed()
        {
            //Nothing extra needed here
        }
    }
}
