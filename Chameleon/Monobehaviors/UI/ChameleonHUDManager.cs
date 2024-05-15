using Chameleon.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Chameleon.Monobehaviors.UI
{
    internal class ChameleonHUDManager : MonoBehaviour
    {
        public SubRoot subRoot;
        public LiveMixin subLiveMixin;
        public BehaviourLOD behaviourLOD;
        public CanvasGroup canvasGroup;
        public FMOD_CustomEmitter creatureDamagesSFX;
        public Image creatureAttackSprite;

        private bool hudActive;
        private bool creatureAttackWarning;
        private float warningAlpha;
        private bool oldWarningState;
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
                UpdateHUD();
                creatureAttackSprite.gameObject.SetActive(creatureAttackWarning);
            }

            if (Player.main.currentSub != subRoot || subRoot.subDestroyed) return;

            float targetAlpha = hudActive ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 4f);
            canvasGroup.interactable = hudActive;

            if (creatureAttackWarning)
            {
                subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.creatureAttackNotification);
                subRoot.subWarning = true; //<-- Come back to later if implementing fire (OR with fireWarning)
            }
            else
            {
                subRoot.subWarning = false;
            }

            warningAlpha = Mathf.PingPong(Time.time * 5f, 1f);
            creatureAttackSprite.color = new Color(1f, 1f, 1f, warningAlpha);

            if(subRoot.subWarning != oldWarningState)
            {
                subRoot.BroadcastMessage("NewAlarmState");
            }
            oldWarningState = subRoot.subWarning;
        }

        private void ClearCreatureWarning()
        {
            creatureAttackWarning = false;
        }

        public void StartPiloting()
        {
            hudActive = true;
        }

        public void StopPiloting()
        {
            hudActive = false;
        }

        public void OnTakeCreatureDamage()
        {
            CancelInvoke(nameof(ClearCreatureWarning));
            Invoke(nameof(ClearCreatureWarning), 10f);
            creatureAttackWarning = true;
            creatureDamagesSFX.Play();
            MainCameraControl.main.ShakeCamera(1.5f);
        }

        private void UpdateHUD()
        {
            foreach (IUIElement element in uIElements)
            {
                element.UpdateUI();
            }
        }

        public void OnChameleonDestroyed()
        {
            foreach (IUIElement element in uIElements)
            {
                element.OnSubDestroyed();
            }
        }
    }
}
