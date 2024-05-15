using Chameleon.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Chameleon.Monobehaviors.UI
{
    internal class HealthUIManager : MonoBehaviour, IUIElement
    {
        public LiveMixin subLiveMixin;
        public Image healthBar;

        public void UpdateUI()
        {
            float healthFraction = subLiveMixin.GetHealthFraction();
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthFraction, Time.deltaTime * 2f);
        }

        public void OnSubDestroyed()
        {
            healthBar.fillAmount = 0;
        }
    }
}
