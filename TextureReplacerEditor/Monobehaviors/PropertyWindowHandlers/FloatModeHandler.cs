﻿using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class FloatModeHandler : PropertyHandler
    {
        public LinkedInputSlider linkedInputSlider;
        private float originalValue;

        public override void SetInfo(Material material, string floatName, object overrideOriginal = null)
        {
            float val = 0;
            if (overrideOriginal == null)
            {
                val = material.GetFloat(floatName);
            }
            else
            {
                val = (float)overrideOriginal;
            }
            
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
                InvokeOnPropertyChanged(new(originalValue, linkedInputSlider.GetCurrentValue(), UnityEngine.Rendering.ShaderPropertyType.Float));
            };

            int valueBounds = val / 10 > 1 ? 100 : 5;
            linkedInputSlider.SetSliderMinValue(-valueBounds);
            linkedInputSlider.SetSliderMaxValue(valueBounds);
        }
    }
}
