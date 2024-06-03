﻿using System;
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
            preciseInputField.text = val.ToString($"F{decimalsToShow}");
        }

        public float GetCurrentValue()
        {
            if(float.TryParse(preciseInputField.text, out float val))
            {
                return val;
            }

            return slider.value;
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
            if(float.TryParse(preciseInputField.text, out float val))
            {
                slider.value = val;
            }
        }

        private void UpdateInputText()
        {
            if (float.Parse(preciseInputField.text) == slider.value) return;

            preciseInputField.text = slider.value.ToString($"F{decimalsToShow}");
        }
    }
}
