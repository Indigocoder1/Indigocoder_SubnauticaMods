using UnityEngine;

namespace ImprovedGravTrap
{
    internal class EnhancedGravSphere : MonoBehaviour
    {
        public StorageContainer container { get; set; }

        public void OnStorageSizeChange()
        {
            container.Resize(Main_Plugin.GravTrapStorageWidth.Value, Main_Plugin.GravTrapStorageHeight.Value);
        }

        private void OnDestroy()
        {
            GravTrap_ModOptions.OnStorageSizeChange -= OnStorageSizeChange;
        }
    }
}
