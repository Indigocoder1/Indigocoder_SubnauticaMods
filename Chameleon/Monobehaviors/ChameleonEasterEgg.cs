using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class ChameleonEasterEgg : MonoBehaviour, IHandTarget
    {
        public Animator animator;
        public FMOD_CustomEmitter emitter;

        public void OnHandClick(GUIHand hand)
        {
            animator.SetTrigger("Activate");
            emitter.Play();
        }

        public void OnHandHover(GUIHand hand)
        {
            
        }
    }
}
