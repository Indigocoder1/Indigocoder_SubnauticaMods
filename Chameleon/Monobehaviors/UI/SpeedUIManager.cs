using UnityEngine;

namespace Chameleon.Monobehaviors.UI
{
    internal class SpeedUIManager : MonoBehaviour
    {
        public Animator animator;
        public CyclopsMotorMode motorMode;

        private bool engineOnLastFrame;

        private void Start()
        {
            animator.SetBool("Active", false);
        }

        private void Update()
        {
            if(motorMode.engineOn != engineOnLastFrame)
            {
                animator.SetBool("Active", motorMode.engineOn);
            }

            engineOnLastFrame = motorMode.engineOn;
        }
    }
}
