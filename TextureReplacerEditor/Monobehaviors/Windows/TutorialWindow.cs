using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class TutorialWindow : DraggableWindow
    {
        public TextMeshProUGUI messageText;
        public Transform linePivot;
        public RectTransform lineImage;

        private Transform lrTarget;

        private void LateUpdate()
        {
            if (!lrTarget)
            {
                linePivot.gameObject.SetActive(false);
                return;
            }

            if(!linePivot.gameObject.activeSelf) linePivot.gameObject.SetActive(true);

            float deltaX = linePivot.position.x - lrTarget.position.x;
            float deltaY = linePivot.position.y - lrTarget.position.y;

            float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;
            angle = (angle + 90) % 360;

            linePivot.transform.eulerAngles = new Vector3(0, 0, angle);

            float distToTarget = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY) * 0.746f;
            lineImage.sizeDelta = new Vector2(lineImage.sizeDelta.x, distToTarget);
        }

        public void SetLRTarget(Transform target)
        {
            lrTarget = target;
        }

        public void SetMessage(string message)
        {
            OpenWindow();
            messageText.text = message;
        }

        public override void CloseWindow()
        {
            InfoMessageWindow messageWindow = TextureReplacerEditorWindow.Instance.messageWindow;
            messageWindow.OpenPrompt($"Are you sure you want to close the tutorial? All progress will be reset.", Color.white, "Yes", "No", OnCloseConfirmed, null);
        }

        private void OnCloseConfirmed()
        {
            TutorialHandler tutorialHandler = TextureReplacerEditorWindow.Instance.tutorialHandler;
            tutorialHandler.RemoveAllItems();
            TextureReplacerEditorWindow.Instance.prefabInfoWindow.EndTutorial();

            TextureReplacerEditorWindow.Instance.materialWindow.SetTutorialHighlightActive(false);
            TextureReplacerEditorWindow.Instance.configViewerWindow.SetTutorialHighlightActive(false);

            base.CloseWindow();
        }
    }
}
