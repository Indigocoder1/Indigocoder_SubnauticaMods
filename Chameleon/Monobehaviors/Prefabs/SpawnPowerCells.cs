using Chameleon.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnPowerCells : MonoBehaviour, ICyclopsReferencer
    {
        private static Dictionary<GameObject, TechType> cachedModels = null;

        public Vector3 spawnedLocalPos;
        public Vector3 spawnedLocalRoation;
        public Vector3 spawnedLocalScale;
        public BatterySource batterySource;

        private void OnValidate()
        {
            if(!batterySource)
            {
                batterySource = GetComponent<BatterySource>();
            }
        }

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            if (cachedModels == null) LoadCellModels(cyclops);
            batterySource = GetComponent<BatterySource>();

            int index = 0;
            foreach (GameObject model in cachedModels.Keys)
            {
                batterySource.batteryModels[index].model = SpawnPowerCellModel(model);
                batterySource.batteryModels[index].techType = cachedModels[model];

                index++;
            }
        }

        private void LoadCellModels(GameObject cyclops)
        {
            GameObject powerCellModel = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/model").gameObject;
            GameObject ionPowerCellModel = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/engine_power_cell_ion").gameObject;

            cachedModels = new()
            {
                { powerCellModel, TechType.PowerCell },
                { ionPowerCellModel, TechType.PrecursorIonPowerCell }
            };
        }

        private GameObject SpawnPowerCellModel(GameObject modelReference)
        {
            GameObject spawnedModel = GameObject.Instantiate(modelReference, transform);

            spawnedModel.transform.localPosition = spawnedLocalPos;
            spawnedModel.transform.localEulerAngles = spawnedLocalRoation;
            spawnedModel.transform.localScale = spawnedLocalScale;

            return spawnedModel;
        }
    }
}
