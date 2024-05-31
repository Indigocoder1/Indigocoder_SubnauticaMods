using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class FloatModeHandler : PropertyHandler
    {
        public LinkedInputSlider linkedInputSlider;

        public override void SetInfo(Material material, string floatName)
        {
            float val = material.GetFloat(floatName);
            linkedInputSlider.SetInitialValue(val);
            linkedInputSlider.OnInputValueChanged += () =>
            {
                material.SetFloat(floatName, linkedInputSlider.GetCurrentValue());
            };

            linkedInputSlider.SetSliderMaxValue(100);
        }
    }
}
