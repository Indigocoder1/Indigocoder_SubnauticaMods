using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class VectorModeHandler : PropertyHandler
    {
        public LinkedInputSlider xSlider;
        public LinkedInputSlider ySlider;
        public LinkedInputSlider zSlider;
        public LinkedInputSlider wSlider;

        private Material material;
        private Vector4 originalVector;
        private string vectorName;

        private Vector4 SliderVector
        {
            get
            {
                return new Vector4(xSlider.GetCurrentValue(), ySlider.GetCurrentValue(), zSlider.GetCurrentValue(), wSlider.GetCurrentValue());
            }
        }

        private void Start()
        {
            xSlider.OnInputValueChanged += OnSliderChanged;
            ySlider.OnInputValueChanged += OnSliderChanged;
            zSlider.OnInputValueChanged += OnSliderChanged;
            wSlider.OnInputValueChanged += OnSliderChanged;
        }

        public override void SetInfo(Material material, string vectorName)
        {
            Vector4 vector = material.GetVector(vectorName);
            originalVector = vector;

            this.material = material;
            InitSliderValues(xSlider, vector.x);
            InitSliderValues(ySlider, vector.y);
            InitSliderValues(zSlider, vector.z);
            InitSliderValues(wSlider, vector.w);
        }

        private void InitSliderValues(LinkedInputSlider slider, float initialValue)
        {
            int valueBounds = initialValue / 10 > 1 ? 100 : 5;

            slider.SetSliderMaxValue(valueBounds);
            slider.SetSliderMinValue(-valueBounds);
            slider.SetInitialValue(initialValue);
            slider.OnInputValueChanged += () => material.SetVector(vectorName, SliderVector);
        }

        private void OnSliderChanged()
        {
            if (!initialized)
            {
                initialized = true;
                return;
            }

            InvokeOnPropertyChanged(new()
            {
                changedType = UnityEngine.Rendering.ShaderPropertyType.Vector,
                originalValue = originalVector,
                newValue = SliderVector
            });
        }
    }
}
