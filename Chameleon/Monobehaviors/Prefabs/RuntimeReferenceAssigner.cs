using Chameleon.Interfaces;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class RuntimeReferenceAssigner : MonoBehaviour, ICyclopsReferencer
    {
        public float waterLevelYOffset = 3;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            var cyclopsConstructing = cyclops.gameObject.GetComponent<VFXConstructing>();
            var vfxConstructing = GetComponent<VFXConstructing>();
            vfxConstructing.ghostMaterial = cyclopsConstructing.ghostMaterial;
            vfxConstructing.alphaTexture = cyclopsConstructing.alphaTexture;
            vfxConstructing.alphaDetailTexture = cyclopsConstructing.alphaDetailTexture;
            vfxConstructing.transparentShaders = cyclopsConstructing.transparentShaders;
            vfxConstructing.surfaceSplashFX = cyclopsConstructing.surfaceSplashFX;

            //GetComponent<PingInstance>().SetType(Plugin.SealPingType);
        }
    }
}
