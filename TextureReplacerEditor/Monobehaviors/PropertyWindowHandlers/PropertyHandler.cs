using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TextureReplacerEditor.Monobehaviors.PropertyWindowHandlers
{
    internal abstract class PropertyHandler : MonoBehaviour
    {
        public static event EventHandler<OnPropertyChangedEventArgs> OnPropertyChanged;
        public abstract void SetInfo(Material material, string propertyName, object overrideValue = null);
        protected bool initialized;

        protected void InvokeOnPropertyChanged(OnPropertyChangedEventArgs args)
        {
            OnPropertyChanged?.Invoke(this, args);
        }
    }

    public class OnPropertyChangedEventArgs : EventArgs
    {
        public object originalValue;
        public object newValue;
        public ShaderPropertyType changedType;

        public OnPropertyChangedEventArgs(object originalValue, object newValue, ShaderPropertyType changedType)
        {
            this.originalValue = originalValue;
            this.newValue = newValue;
            this.changedType = changedType;
        }
    }
}
