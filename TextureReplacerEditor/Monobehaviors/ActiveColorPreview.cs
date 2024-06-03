using System;
using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class ActiveColorPreview : MonoBehaviour
    {
        public event Action OnColorChanged;
        public LinkedInputSlider redSlider;
        public LinkedInputSlider greenSlider;
        public LinkedInputSlider blueSlider;
        public Image colorPreview;
        private Color color;
        private int currentIgnoreOnChangeCount;

        private void Start()
        {
            redSlider.OnInputValueChanged += UpdateColorPreview;
            greenSlider.OnInputValueChanged += UpdateColorPreview;
            blueSlider.OnInputValueChanged += UpdateColorPreview;
        }

        public void SetActiveColor(Color color)
        {
            currentIgnoreOnChangeCount = 0;
            redSlider.SetInitialValue(color.r);
            greenSlider.SetInitialValue(color.g);
            blueSlider.SetInitialValue(color.b);
            this.color = color;

            if(colorPreview)
            {
                colorPreview.color = color;
            }
        }

        public Color GetCurrentColor()
        {
            return colorPreview.color;
        }

        private void UpdateColorPreview()
        {
            if (color == null) return;

            colorPreview.color = new Color(redSlider.slider.value, greenSlider.slider.value, blueSlider.slider.value);

            if(currentIgnoreOnChangeCount <= 1)
            {
                currentIgnoreOnChangeCount++;
                return;
            }
            OnColorChanged?.Invoke();
        }
    }
}
