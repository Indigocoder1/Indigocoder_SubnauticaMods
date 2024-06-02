using System.Collections.Generic;
using TextureReplacerEditor.Monobehaviors.Windows;
using UnityEngine;
using UnityEngine.UI;

using ConfigInfo = TextureReplacer.CustomTextureReplacer.ConfigInfo;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class CustomConfigItem : MonoBehaviour
    {
        public ConfigInfo configInfo { get; private set; }
        //public List<>

        public Toggle isVariationToggle;
        public LinkedInputSlider variationChanceSlider;

        private void Start()
        {
            isVariationToggle.onValueChanged.AddListener((_) => UpdateSliderInteractable());
        }
        
        public void SetInfo(ConfigInfo info)
        {
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
