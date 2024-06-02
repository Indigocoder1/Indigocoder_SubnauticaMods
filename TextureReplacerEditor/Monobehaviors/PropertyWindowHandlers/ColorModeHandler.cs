using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class ColorModeHandler : PropertyHandler
    {
        public ActiveColorPreview activeColorPreview;
        private Color originalColor;

        public override void SetInfo(Material material, string colorName)
        {
            Color color = material.GetColor(colorName);
            activeColorPreview.SetActiveColor(color);
            originalColor = color;

            activeColorPreview.OnColorChanged += () =>
            {
                if (!initialized)
                {
                    initialized = true;
                    return;
                }

                material.SetColor(colorName, activeColorPreview.GetCurrentColor());
                InvokeOnPropertyChanged(new()
                {
                    changedType = UnityEngine.Rendering.ShaderPropertyType.Color,
                    originalValue = originalColor,
                    newValue = activeColorPreview.GetCurrentColor()
                });
            };
        }
    }
}
