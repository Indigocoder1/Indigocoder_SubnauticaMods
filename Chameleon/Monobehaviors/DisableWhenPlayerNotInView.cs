using UnityEngine;
using UWE;

namespace Chameleon.Monobehaviors
{
    internal class DisableWhenPlayerNotInView : MonoBehaviour
    {
        [Range(-1, 1)]
        public float fovAllowed;

        private MeshRenderer rend;

        private void Start ()
        {
            rend = GetComponent<MeshRenderer>();
            //SubEnterHandTarget.OnHatchUsed += UpdateHatchVisibility;
        }

        private void Update()
        {
            UpdateHatchVisibility();
        }

        private void UpdateHatchVisibility()
        {
            Vector3 dirToPlayer = Camera.main.transform.position - transform.position;
            float dot = Vector3.Dot(-transform.up, dirToPlayer.normalized);
            float multiplier = fovAllowed >= 0 ? 1 : -1;
            if(multiplier * dot < fovAllowed)
            {
                rend.enabled = false;
            }
            else
            {
                rend.enabled = true;
            }
        }

        private void OnDestroy()
        {
            //SubEnterHandTarget.OnHatchUsed -= UpdateHatchVisibility;
        }
    }
}
