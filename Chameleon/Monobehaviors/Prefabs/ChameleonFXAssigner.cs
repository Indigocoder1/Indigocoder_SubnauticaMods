using Chameleon.Interfaces;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class ChameleonFXAssigner : MonoBehaviour, ICyclopsReferencer
    {
        public VFXController controller;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            VFXController cyclopsController = cyclops.transform.Find("FX/CyclopsExplosionFX").GetComponent<VFXController>();
            controller.emitters[0].instanceGO = cyclopsController.emitters[0].instanceGO;
            controller.emitters[1].instanceGO = cyclopsController.emitters[1].instanceGO;
        }
    }
}
