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
        }

        private void Update()
        {
            pingInstance.visible = !pickupable.attached;
        }
    }
}
