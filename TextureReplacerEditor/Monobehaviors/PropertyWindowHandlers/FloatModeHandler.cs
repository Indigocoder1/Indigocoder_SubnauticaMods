using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class FloatModeHandler : PropertyHandler
    {
        public LinkedInputSlider linkedInputSlider;
        private float originalValue;

        public override void SetInfo(Material material, string floatName)
        {
            float val = material.GetFloat(floatName);
            linkedInputSlider.SetInitialValue(val);
            originalValue = val;

            linkedInputSlider.OnInputValueChanged += () =>
            {
                material.SetFloat(floatName, linkedInputSlider.GetCurrentValue());
                InvokeOnPropertyChanged(new()
                {
                    changedType = UnityEngine.Rendering.ShaderPropertyType.Float,
                    previousValue = originalValue,
                    newValue = linkedInputSlider.GetCurrentValue()
                });
            };

            linkedInputSlider.SetSliderMaxValue(100);
        }
    }
}
