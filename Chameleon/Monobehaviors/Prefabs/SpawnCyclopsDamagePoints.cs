using Chameleon.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnCyclopsDamagePoints : MonoBehaviour, ICyclopsReferencer
    {
        public CyclopsExternalDamageManager damageManager;

        private static CyclopsExternalDamageManager cyclopsManager;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            cyclopsManager = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>();
            List<CyclopsDamagePoint> points = new();

            foreach (var slot in gameObject.GetComponentsInChildren<DamagePointSlot>())
            {
                if (slot.damagePrefabIndex <= -1) slot.damagePrefabIndex = Random.Range(0, cyclopsManager.damagePoints.Length);

                var prefab = cyclopsManager.damagePoints[slot.damagePrefabIndex].gameObject;
                var copy = Instantiate(prefab, slot.transform.position, slot.transform.rotation, slot.transform);

                points.Add(copy.GetComponent<CyclopsDamagePoint>());
            }

            damageManager.damagePoints = points.ToArray();
        }
    }
}
