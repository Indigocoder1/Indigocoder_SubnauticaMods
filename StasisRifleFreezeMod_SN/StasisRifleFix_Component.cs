using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace StasisRifleFix
{
    internal class StasisRifleFix_Component : MonoBehaviour
    {
        private Creature creature;
        private float previousAggression;
        private bool isFrozen;

        private void Start()
        {
            creature = GetComponentInParent<Creature>();

            if (creature == null)
            {
                Logger.Log(Logger.Level.Error, "Creature was not able to be found!");
            }
        }

        public void OnFreeze()
        {
            previousAggression = creature.Aggression.Value;
            creature.Aggression.Value = 0;
            creature.GetAnimator().enabled = false;
            isFrozen = true;
        }

        public void OnUnfreeze()
        {
            creature.Aggression.Value = previousAggression;
            creature.GetAnimator().enabled = true;
            creature.UpdateBehaviour(Time.deltaTime);
            isFrozen = false;
        }

        public bool IsFrozen()
        {
            return isFrozen;
        }
    }
}
