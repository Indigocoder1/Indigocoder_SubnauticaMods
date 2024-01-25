using System.Collections.Generic;
using UnityEngine;

namespace StasisRifleFixMod_BepInEx
{
    internal class StasisRifleFix_Component : MonoBehaviour
    {
        public Creature creature;
        private float previousAggression;
        private bool isFrozen;

        private FMOD_CustomEmitter[] emitters;
        private Dictionary<FMOD_CustomEmitter, bool> emitterStates = new Dictionary<FMOD_CustomEmitter, bool>();

        private void Start()
        {
            emitters = GetComponentsInChildren<FMOD_CustomEmitter>(true);
        }

        private void FixedUpdate()
        {
            if (!creature.liveMixin)
            {
                return;
            }

            if (!creature.liveMixin.IsAlive())
            {
                if(creature.GetAnimator())
                {
                    creature.GetAnimator().enabled = true;
                }
                isFrozen = false;
            }
        }

        private void LateUpdate()
        {
            if (!isFrozen)
            {
                return;
            }

            creature.Aggression.Value = 0;
        }

        public void OnFreezeByStasisSphere()
        {
            previousAggression = creature.Aggression.Value;
            creature.Aggression.Value = 0;
            creature.GetAnimator().enabled = false;
            isFrozen = true;

            foreach (FMOD_CustomEmitter emitter in emitters)
            {
                emitterStates[emitter] = emitter.enabled;
                emitter.enabled = false;
            }
        }

        public void OnUnfreezeByStasisSphere()
        {
            isFrozen = false;
            creature.Aggression.Value = previousAggression;
            if(creature.GetAnimator()) creature.GetAnimator().enabled = true;

            creature.UpdateBehaviour(Time.time, Time.deltaTime);

            foreach (FMOD_CustomEmitter emitter in emitters)
            {
                emitter.enabled = emitterStates[emitter];
            }
        }

        public bool IsFrozen()
        {
            return isFrozen;
        }
    }
}
