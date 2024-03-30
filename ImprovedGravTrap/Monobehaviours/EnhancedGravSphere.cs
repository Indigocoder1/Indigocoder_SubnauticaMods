using UnityEngine;

namespace ImprovedGravTrap
{
    internal class EnhancedGravSphere : MonoBehaviour
    {
        public StorageContainer container { get; set; }
        [SerializeField] public PingInstance pingInstance { get; set; }
        [SerializeField] private Pickupable pickupable;

        private void Start() 
        {
            pickupable = GetComponent<Pickupable>();
            pingInstance = GetComponent<PingInstance>();

            pingInstance.SetLabel("Enhanced Grav Trap");

            StorageContainer storageContainer = GetComponentInChildren<StorageContainer>();
            Main_Plugin.logger.LogInfo($"Container = {storageContainer.container}");
            storageContainer.container.isAllowedToAdd = (p, v) =>
            {
                if (p.GetTechType() == ImprovedTrap_Craftable.techType) return false;

                return true;
            };
        }

        private void Update()
        {
            pingInstance.visible = !pickupable.attached;
        }
    }
}
