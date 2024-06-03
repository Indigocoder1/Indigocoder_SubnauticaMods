using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class InfoMessageWindow : DraggableWindow
    {
        public TextMeshProUGUI messageText;
        public Button confirmButton;
        public TextMeshProUGUI confirmText;
        public Button denyButton;
        public TextMeshProUGUI denyText;

        private Action onConfirm;
        private Action onDeny;

        private void Start()
        {
            confirmButton.onClick.AddListener(OnConfirmClick);
            denyButton.onClick.AddListener(OnDenyClick);
        }

        public void OpenMessage(string text, Color color)
        {
            OpenWindow();

            messageText.text = text;
            messageText.color = color;

            confirmButton.gameObject.SetActive(true);
            denyButton.gameObject.SetActive(false);

            confirmText.text = "Ok";
        }

        public void OpenPrompt(string text, Color color, string confirmText, string denyText, Action onConfirm, Action onDeny)
        {
            OpenWindow();

            this.onConfirm = onConfirm;
            this.onDeny = onDeny;

            messageText.text = text;
            messageText.color = color;

            confirmButton.gameObject.SetActive(true);
            denyButton.gameObject.SetActive(true);

            this.confirmText.text = confirmText;
            this.denyText.text = denyText;
        }

        private void OnConfirmClick()
        {
            onConfirm?.Invoke();
        }

        private void OnDenyClick() 
        {
            onDeny?.Invoke();
        }
    }
}
