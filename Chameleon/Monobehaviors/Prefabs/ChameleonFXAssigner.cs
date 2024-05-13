using Chameleon.Interfaces;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class ChameleonFXAssigner : MonoBehaviour, ICyclopsReferencer
    {
        public VFXController controller;
        public Vector3 interiorExplosionScale = Vector3.one;
        public Vector3 exteriorExplosionScale = Vector3.one;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            VFXController cyclopsController = cyclops.transform.Find("FX/CyclopsExplosionFX").GetComponent<VFXController>();
            controller.emitters[0].fx = cyclopsController.emitters[0].fx;
            controller.emitters[1].fx = cyclopsController.emitters[1].fx;

            controller.emitters[0].fx.transform.localScale = interiorExplosionScale;
            controller.emitters[1].fx.transform.localScale = exteriorExplosionScale;
        }
    }
}
