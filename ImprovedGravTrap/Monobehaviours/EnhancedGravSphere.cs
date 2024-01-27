using System;
using UnityEngine;

namespace ImprovedGravTrap
{
    internal class EnhancedGravSphere : MonoBehaviour
    {
        public StorageContainer container { get; set; }

        /*
        public void OnStorageSizeChange(object sender, EventArgs e)
        {
            container.Resize(Main_Plugin.GravTrapStorageWidth.Value, Main_Plugin.GravTrapStorageHeight.Value);
            Main_Plugin.logger.LogInfo($"Container = {container} (Width = {container.width}, Height = {container.height})");
        }

        private void OnDestroy()
        {
            GravTrap_ModOptions.OnStorageSizeChange -= OnStorageSizeChange;
        }
        */
    }
}
