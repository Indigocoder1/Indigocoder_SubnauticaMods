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
    }
}
