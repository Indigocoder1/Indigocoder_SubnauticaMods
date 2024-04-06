using UnityEngine;

namespace ImprovedGravTrap
{
    internal class EnhancedGravSphere : MonoBehaviour
    {
        public StorageContainer container { get; set; }
        [SerializeField] public PingInstance pingInstance { get; set; }
        [SerializeField] private Pickupable pickupable;

        private Gravsphere gravSphere;
        private GameObject model;
        private Animator animator;
        private bool inRangeActivated;

        private void Start() 
        {
            pickupable = GetComponent<Pickupable>();
            pingInstance = GetComponent<PingInstance>();
            gravSphere = GetComponent<Gravsphere>();
            animator = GetComponentInChildren<Animator>();

            model = transform.Find("gravSphere_anim").gameObject;

            pingInstance.SetLabel("Enhanced Grav Trap");

            StorageContainer storageContainer = GetComponentInChildren<StorageContainer>();

            storageContainer.container.isAllowedToAdd = (p, v) =>
            {
                if (p.GetTechType() == ImprovedTrap_Craftable.techType) return false;

                return true;
            };
        }

        private void Update()
        {
            pingInstance.visible = !pickupable.attached;

            if (!gravSphere.pickupable.isValidHandTarget) return;

            if((transform.position - Player.main.transform.position).sqrMagnitude > 10000 && gameObject.activeSelf)
            {
                inRangeActivated = false;

                gravSphere.enabled = false;
                model.SetActive(false);            
            }
            else if(!inRangeActivated)
            {
                inRangeActivated = true;

                gravSphere.enabled = true;
                gravSphere.trigger.enabled = true;
                gravSphere.pickupable.attached = false;

                model.SetActive(true);
                animator.SetBool("deployed", true);
            }
        }
    }
}
