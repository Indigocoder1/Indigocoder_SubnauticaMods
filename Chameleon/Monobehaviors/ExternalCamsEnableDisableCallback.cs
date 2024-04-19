using UnityEngine;
using UnityEngine.Events;

namespace Chameleon.Monobehaviors
{
    internal class ExternalCamsEnableDisableCallback : MonoBehaviour
    {
        public UnityEvent onCamerasActivated;
        public UnityEvent onCamerasDeactivated;

        private CyclopsExternalCams externalCams;
        private bool activeLastFrame;

        private void Start()
        {
            externalCams = GetComponent<CyclopsExternalCams>();
        }

        private void Update()
        {
            if(externalCams.active != activeLastFrame)
            {
                switch(externalCams.active)
                {
                    case true:
                        onCamerasActivated.Invoke();
                        break;
                    case false:
                        onCamerasDeactivated.Invoke();
                        break;
                }
            }

            activeLastFrame = externalCams.active;
        }
    }
}
