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
                if (!initialized)
                {
                    initialized = true;
                    return;
                }

                material.SetFloat(floatName, linkedInputSlider.GetCurrentValue());
                InvokeOnPropertyChanged(new()
                {
                    changedType = UnityEngine.Rendering.ShaderPropertyType.Float,
                    originalValue = originalValue,
                    newValue = linkedInputSlider.GetCurrentValue()
                });
            };

            int valueBounds = val / 10 > 1 ? 100 : 5;
            linkedInputSlider.SetSliderMinValue(-valueBounds);
            linkedInputSlider.SetSliderMaxValue(valueBounds);
        }
    }
}
