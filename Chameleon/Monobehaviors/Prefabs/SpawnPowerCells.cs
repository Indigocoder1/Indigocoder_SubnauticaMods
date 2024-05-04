using Chameleon.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnPowerCells : MonoBehaviour, ICyclopsReferencer
    {
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
            var models =  LoadCellModels(cyclops);
            batterySource = GetComponent<BatterySource>();

            for (int i = 0; i < models.Length; i++)
            {
                batterySource.batteryModels[i].model = SpawnPowerCellModel(models[i]);
            }
        }

        private GameObject[] LoadCellModels(GameObject cyclops)
        {
            var powerCellModel = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/model").gameObject;
            var ionPowerCellModel = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/engine_power_cell_ion").gameObject;

            return new[]
            {
                powerCellModel,
                ionPowerCellModel
            };
        }

        private GameObject SpawnPowerCellModel(GameObject modelReference)
        {
            GameObject spawnedModel = Instantiate(modelReference, transform);

            spawnedModel.transform.localPosition = spawnedLocalPos;
            spawnedModel.transform.localEulerAngles = spawnedLocalRoation;
            spawnedModel.transform.localScale = spawnedLocalScale;

            return spawnedModel;
        }
    }
}
