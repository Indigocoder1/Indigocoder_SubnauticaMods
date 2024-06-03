using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers
{
    internal abstract class ConfigChangeHandler : MonoBehaviour
    {
        public abstract void SetInfo(object original, object changed); 
    }
}
