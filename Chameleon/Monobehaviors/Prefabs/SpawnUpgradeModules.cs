using Chameleon.Monobehaviors.Cyclops;
using System;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnUpgradeModules : MonoBehaviour, ICyclopsReferencer
    {
        public Transform[] moduleSlots;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            GameObject upgradeModuleModel = cyclops.transform
                .Find("CyclopsMeshStatic/undamaged/cyclops_LOD0/cyclops_engine_room/cyclops_engine_console/" +
                "Submarine_engine_GEO/submarine_engine_console_01_wide/engine_console_key_01_01")
                .gameObject;

            foreach (Transform slot in moduleSlots)
            {
                GameObject clone = GameObject.Instantiate(upgradeModuleModel, slot, false);
                clone.transform.localPosition = new Vector3(-0.41f, -0.69f, -2.69f);
                clone.transform.localEulerAngles = Vector3.zero;
                clone.transform.localScale = Vector3.one * 2.7f;
                clone.gameObject.SetActive(true);
            }
        }
    }
}
