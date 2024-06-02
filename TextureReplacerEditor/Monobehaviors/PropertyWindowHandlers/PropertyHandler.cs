using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal abstract class PropertyHandler : MonoBehaviour
    {
        public virtual event EventHandler<OnPropertyChangedEventArgs> OnPropertyChanged;
        public abstract void SetInfo(Material material, string propertyName);

        protected void InvokeOnPropertyChanged(OnPropertyChangedEventArgs args)
        {
            OnPropertyChanged?.Invoke(this, args);
        }
    }

    public class OnPropertyChangedEventArgs : EventArgs
    {
        public object sender;
        public object previousValue;
        public object newValue;
        public int materialIndex;
        public ShaderPropertyType changedType;
    }
}
