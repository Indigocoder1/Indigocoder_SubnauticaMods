﻿using System.Collections.Generic;
using TextureReplacerEditor.Monobehaviors.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TextureReplacerEditor.Monobehaviors.Windows.MaterialWindow;
using ConfigInfo = TextureReplacer.CustomTextureReplacer.ConfigInfo;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class CustomConfigItem : MonoBehaviour
    {
        public ConfigInfo configInfo { get; private set; }
        public List<PropertyEditData> propertyEdits { get; private set; }

        public TextMeshProUGUI configNameText;
        public Toggle isVariationToggle;
        public LinkedInputSlider variationChanceSlider;

        private void Start()
        {
            isVariationToggle.onValueChanged.AddListener((_) => UpdateSliderInteractable());
            isVariationToggle.onValueChanged.AddListener((_) => configInfo.isVariation = isVariationToggle.isOn);
            variationChanceSlider.OnInputValueChanged += () => configInfo.variationChance = variationChanceSlider.GetCurrentValue() / 100f;

            UpdateSliderInteractable();
        }

        public void SetInfo(ConfigInfo info, List<PropertyEditData> edits)
        {
            configNameText.text = info.configName;
            propertyEdits = edits;
            configInfo = info;
            isVariationToggle.isOn = configInfo.isVariation;
            variationChanceSlider.SetInitialValue(configInfo.variationChance);
        }

        public void SetCurrentConfig()
        {
            TextureReplacerEditorWindow.Instance.configViewerWindow.SetCurrentConfig(this);
        }

        private void UpdateSliderInteractable()
        {
            variationChanceSlider.slider.interactable = isVariationToggle.isOn;
            variationChanceSlider.preciseInputField.interactable = isVariationToggle.isOn;
        }
    }
}
