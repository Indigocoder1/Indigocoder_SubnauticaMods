using Chameleon.Monobehaviors.Cyclops;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnPowerCells : MonoBehaviour, ICyclopsReferencer
    {
        private static GameObject[] cachedModels = null;

        public Vector3 spawnedLocalPos;
        public Vector3 spawnedLocalRoation;
        public Vector3 spawnedLocalScale;
        private BatterySource batterySource;

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

            for (int i = 0; i < cachedModels.Length; i++)
            {
                batterySource.batteryModels[i].model = SpawnPowerCellModel(cachedModels[i]);
            }
        }

        private void LoadCellModels(GameObject cyclops)
        {
            GameObject powerCellModel = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/model").gameObject;
            GameObject ionPowerCellModel = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/engine_power_cell_ion").gameObject;

            cachedModels = new[]
            {
                powerCellModel,
                ionPowerCellModel
            };
        }

        private GameObject SpawnPowerCellModel(GameObject modelReference)
        {
            GameObject spawnedModel = GameObject.Instantiate(modelReference);

            spawnedModel.transform.localPosition = spawnedLocalPos;
            spawnedModel.transform.localEulerAngles = spawnedLocalRoation;
            spawnedModel.transform.localScale = spawnedLocalScale;

            return spawnedModel;
        }
    }
}
