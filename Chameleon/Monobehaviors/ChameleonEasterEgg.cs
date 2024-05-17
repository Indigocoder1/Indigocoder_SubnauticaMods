using Chameleon.ScriptableObjects;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class ChameleonEasterEgg : MonoBehaviour
    {
        public Animator animator;
        public FMOD_CustomEmitter emitter;

        private void OnMouseDown()
        {
            animator.SetTrigger("Activate");
            emitter.Play();
        }
    }
}
