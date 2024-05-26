using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TodoList.Monobehaviors
{
    internal class TodoInputField : MonoBehaviour
    {
        private const float ALPHA_TRANSITION_SPEED = 12f;
        internal static List<TodoInputField> inputFields = new();

        public bool IsChecked
        {
            get
            {
                return toggle.isOn;
            }
        }

        public TMP_InputField inputField;
        private Toggle toggle;
        private TextMeshProUGUI text;
        private bool wasFocusedLastFrame;

        private CanvasGroup canvasGroup;
        private float targetAlpha = 1f;

        private void Start()
        {
            inputField = GetComponent<TMP_InputField>();
            canvasGroup = GetComponent<CanvasGroup>();
            toggle = transform.parent.GetComponentInChildren<Toggle>();
            text = (TextMeshProUGUI)inputField.textComponent;

            toggle.onValueChanged.AddListener((bool val) => OnToggleChanged(val));
        }

        private void Update()
        {
            if (inputField.isFocused != wasFocusedLastFrame)
            {
                FPSInputModule.current.lockMovement = inputField.isFocused;
            }

            wasFocusedLastFrame = inputField.isFocused;

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * ALPHA_TRANSITION_SPEED);
        }

        private void OnToggleChanged(bool value)
        {
            inputField.interactable = !value;
            targetAlpha = value ? 0.2f : 1f;
        }

        private void OnEnable()
        {
            if(!inputFields.Contains(this))
            {
                inputFields.Add(this);
            }
        }

        private void OnDisable()
        {
            if (inputFields.Contains(this))
            {
                inputFields.Remove(this);
            }
        }
    }
}
