using Chameleon.Interfaces;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class DamageManagerPrefabSetter : MonoBehaviour, ICyclopsReferencer
    {
        public CyclopsExternalDamageManager manager;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            manager.fxPrefabs = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
        }
    }
}
