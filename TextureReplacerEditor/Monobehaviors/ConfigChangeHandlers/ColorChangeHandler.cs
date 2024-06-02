using UnityEngine;
using UnityEngine.UI;

namespace TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers
{
    internal class ColorChangeHandler : ConfigChangeHandler
    {
        public Image oldColorImage;
        public Image newColorImage;

        public override void SetInfo(object original, object changed)
        {
            oldColorImage.color = (Color)original;
            newColorImage.color = (Color)changed;
        }
    }
}
