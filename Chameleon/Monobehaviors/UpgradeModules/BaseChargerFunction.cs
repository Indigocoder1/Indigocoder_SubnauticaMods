using UnityEngine;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    internal abstract class BaseChargerFunction : MonoBehaviour
    {
        public virtual float updateCooldown => -1f;
        public int installedModules;
        protected PowerRelay relay;

        public virtual void Awake()
        {
            relay = GetComponentInParent<PowerRelay>();
            if(updateCooldown > 0f)
            {
                InvokeRepeating(nameof(UpdateCharge), 1f, updateCooldown);
            }
        }

        public virtual void Update()
        {
            if (updateCooldown <= 0f)
            {
                UpdateCharge();
            }
        }

        public void UpdateCharge()
        {
            if (installedModules <= 0) return;

            relay.AddEnergy(GetCharge() * installedModules, out _);
        }

        public abstract float GetCharge();
    }
}
