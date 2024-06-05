using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class BlinkingHighlight : MonoBehaviour
    {
        private Image image;
        private float currentSinVal;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            currentSinVal += Time.unscaledDeltaTime;
            currentSinVal %= Mathf.PI;

            float alpha = Mathf.Sin(currentSinVal) / 2;
            alpha = Mathf.Clamp01(alpha);
            Color col = image.color;
            col.a = alpha;
            image.color = col;
        }
    }
}
