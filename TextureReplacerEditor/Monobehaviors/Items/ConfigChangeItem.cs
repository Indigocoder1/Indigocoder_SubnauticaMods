using TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Items
{
    internal class ConfigChangeItem : MonoBehaviour
    {
        public ConfigChangeHandler textureChangeHandler;
        public ConfigChangeHandler colorChangeHandler;
        public ConfigChangeHandler floatChangeHandler;
        public ConfigChangeHandler configChangeHandler;

        public void SetInfo()
        {

        }
    }
}
