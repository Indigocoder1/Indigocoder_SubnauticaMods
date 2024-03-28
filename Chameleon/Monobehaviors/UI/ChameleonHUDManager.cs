using Chameleon.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chameleon.Monobehaviors.UI
{
    internal class ChameleonHUDManager : MonoBehaviour
    {
        public SubRoot subRoot;
        public LiveMixin subLiveMixin;
        public BehaviourLOD behaviourLOD;
        public CanvasGroup canvasGroup;

        private bool hudActive;
        private List<IUIElement> uIElements;

        private void OnValidate()
        {
            if (!subRoot) subRoot = GetComponentInParent<SubRoot>();
            if (!subLiveMixin) subLiveMixin = GetComponentInParent<LiveMixin>();
            if (!behaviourLOD) behaviourLOD = GetComponentInParent<BehaviourLOD>();
        }

        private void Start()
        {
            canvasGroup.alpha = 0f;
            uIElements = GetComponentsInChildren<IUIElement>().ToList();
        }

        private void Update()
        {
            if (!behaviourLOD.IsFull()) return;

            if(subLiveMixin.IsAlive())
            {
                foreach(IUIElement element in uIElements)
                {
                    element.UpdateUI();
                }
            }

            if (Player.main.currentSub != subRoot || subRoot.subDestroyed) return;

            float targetAlpha = hudActive ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 3f);
            canvasGroup.interactable = hudActive;
        }

        public void StartPiloting()
        {
            hudActive = true;
        }

        public void StopPiloting()
        {
            hudActive = false;
        }

        public void OnTakeCollisionDamage(float value)
        {
            value *= 1.5f;
            value = Mathf.Clamp(value / 100f, 0.5f, 1.5f);
            MainCameraControl.main.ShakeCamera(value);
        }
    }
}
