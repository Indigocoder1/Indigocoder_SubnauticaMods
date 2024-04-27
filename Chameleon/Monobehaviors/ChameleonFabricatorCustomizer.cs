using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class ChameleonFabricatorCustomizer : MonoBehaviour
    {
        private Fabricator fabricator;

        public void Start()
        {
            fabricator = transform.GetChild(0).gameObject.GetComponent<Fabricator>();
            fabricator.handOverText = Language.main.Get("ChameleonFabricatorHandOver");
            fabricator.craftTree = Main_Plugin.ChameleonFabricatorTree;
        }
    }
}
