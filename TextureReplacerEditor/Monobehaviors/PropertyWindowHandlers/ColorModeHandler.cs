using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal class ColorModeHandler : PropertyHandler
    {
        public ActiveColorPreview activeColorPreview;
        private Color originalColor;
        private Material material;

        public override void SetInfo(Material material, string colorName, object overrideOriginal = null)
        {
            this.material = material;
            propertyName = colorName;

            Color color = Color.white;
            if (overrideOriginal == null)
            {
                color = material.GetColor(colorName);
            }
            else
            {
                color = (Color)overrideOriginal;
            }
             
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
                InvokeOnPropertyChanged(new(originalColor, activeColorPreview.GetCurrentColor(), UnityEngine.Rendering.ShaderPropertyType.Color));
            };
        }

        public void UpdateColor()
        {
            Color color = material.GetColor(propertyName);
            if (activeColorPreview.GetCurrentColor().Equals(color)) return;

            activeColorPreview.SetActiveColor(color);
        }

        public override void UpdateMaterial()
        {
            Color color = material.GetColor(propertyName);
            if (activeColorPreview.GetCurrentColor().Equals(color)) return;

            activeColorPreview.SetActiveColor(color);
        }
    }
}
