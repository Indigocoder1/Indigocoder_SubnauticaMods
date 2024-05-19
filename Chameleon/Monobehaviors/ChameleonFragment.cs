using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class ChameleonFragment : MonoBehaviour
    {
        public GameObject easterEgg;

        public void SetEasterEggActive(bool active)
        {
            easterEgg.SetActive(active);
        }

        public void SetupResourceTracker()
        {
            var tracker = GetComponent<ResourceTracker>();
            tracker.prefabIdentifier = GetComponent<PrefabIdentifier>();
        }
    }
}
