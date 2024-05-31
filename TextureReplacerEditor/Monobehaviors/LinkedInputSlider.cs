using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class LinkedInputSlider : MonoBehaviour
    {
        public event Action OnInputValueChanged;

        public Slider slider;
        public TMP_InputField preciseInputField;
        public int decimalsToShow;

        private void Start()
        {
            slider.onValueChanged.AddListener((_) =>
            {
                OnInputValueChanged?.Invoke();
                UpdateInputText();
            });

            preciseInputField.onValueChanged.AddListener((_) =>
            {
                OnInputValueChanged?.Invoke();
                UpdateSliderVal();
            });

            preciseInputField.text = slider.value.ToString($"F{decimalsToShow}");
        }

        public void SetInitialValue(float val)
        {
            slider.value = val;
            preciseInputField.text = val.ToString();
        }

        public float GetCurrentValue()
        {
            return float.Parse(preciseInputField.text);
        }

        public void SetSliderMaxValue(float max)
        {
            slider.maxValue = max;
        }

        public void SetSliderMinValue(float min)
        {
            slider.minValue = min;
        }

        private void UpdateSliderVal()
        {
            slider.value = float.Parse(preciseInputField.text);
        }

        private void UpdateInputText()
        {
            if (float.Parse(preciseInputField.text) == slider.value) return;

            preciseInputField.text = slider.value.ToString($"F{decimalsToShow}");
        }
    }
}
