using Chameleon.Interfaces;
using UnityEngine;

namespace Chameleon.Monobehaviors.UI
{
    internal class EngineUIManager : MonoBehaviour, IUIElement
    {
        public CyclopsMotorMode motorMode;
        public GameObject engineOffIndicator;
        //public Animator speedModeAnimator;

        public void UpdateUI()
        {
            engineOffIndicator.SetActive(!motorMode.engineOn);
        }
    }
}
