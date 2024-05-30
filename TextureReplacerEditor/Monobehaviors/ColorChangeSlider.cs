using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class ColorChangeSlider : MonoBehaviour
    {
        public Slider slider { get; private set; }

        private void Start()
        {
            slider = GetComponent<Slider>();
        }

        public void SetInitialValue(float val)
        {
            slider.value = val;
        }
    }
}
