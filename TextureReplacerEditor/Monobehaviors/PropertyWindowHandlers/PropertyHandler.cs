using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal abstract class PropertyHandler : MonoBehaviour
    {
        public abstract void SetInfo(Material material, string propertyName);
    }
}
