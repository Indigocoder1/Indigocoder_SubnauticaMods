using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class MaterialItem : MonoBehaviour
    {
        public ColorChangeSlider redSlider;
        public ColorChangeSlider greenSlider;
        public ColorChangeSlider blueSlider;
        public Image colorPreview;

        private Material material;

        public void SetInfo(Material material)
        {
            this.material = material;

            redSlider.slider.onValueChanged.AddListener((e) => OnSliderChanged());
            greenSlider.slider.onValueChanged.AddListener((e) => OnSliderChanged());
            blueSlider.slider.onValueChanged.AddListener((e) => OnSliderChanged());

            redSlider.SetInitialValue(material.color.r);
            greenSlider.SetInitialValue(material.color.g);
            blueSlider.SetInitialValue(material.color.b);

            UpdateColorPreview();
        }

        private void OnSliderChanged()
        {
            UpdateColorPreview();
        }

        private void UpdateColorPreview()
        {
            colorPreview.color = new Color(redSlider.slider.value, greenSlider.slider.value, blueSlider.slider.value);
        }
    }
}
