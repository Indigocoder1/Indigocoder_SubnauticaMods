using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class DamagePointSlot : MonoBehaviour
    {
        [Tooltip("The specific child index of cyclops damage prefabs to use for this object.\nUse -1 to pick a random one")]
        public int damagePrefabIndex = -1;
    }
}
