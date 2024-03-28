using Chameleon.Interfaces;
using Chameleon.Monobehaviors.Tags;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnFloodlights : MonoBehaviour, ICyclopsReferencer
    {
        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            Transform cyclopsFloodlights = cyclops.transform.Find("Floodlights");

            foreach (FloodlightMarker marker in GetComponentsInChildren<FloodlightMarker>(true))
            {
                Transform lightPrefab = cyclopsFloodlights.Find(marker.lightPrefabObjectName);
                if(!lightPrefab)
                {
                    Main_Plugin.logger.LogError($"{marker.name} has an invalid light prefab name. Can't find {marker.lightPrefabObjectName} in cyclops floodlights");
                    continue;
                }

                Instantiate(lightPrefab, marker.transform.position, marker.transform.rotation, marker.transform).gameObject.SetActive(true);
            }
        }
    }
}
