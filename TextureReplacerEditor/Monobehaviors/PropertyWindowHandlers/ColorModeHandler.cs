using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class ColorModeHandler : PropertyHandler
    {
        public ActiveColorPreview activeColorPreview;

        public override void SetInfo(Material material, string colorName)
        {
            Color color = material.GetColor(colorName);
            activeColorPreview.SetActiveColor(color);
            activeColorPreview.OnColorChanged += () =>
            {
                material.SetColor(colorName, activeColorPreview.GetCurrentColor());
            };
        }
    }
}
