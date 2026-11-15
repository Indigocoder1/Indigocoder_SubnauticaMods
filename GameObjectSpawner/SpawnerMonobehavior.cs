using Nautilus.Utility;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GameObjectSpawner
{
    internal class SpawnerMonobehavior : MonoBehaviour
    {
        private static GameObject prefab;

        private void Start()
        {
            GetPrefab();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(Plugin.spawnKey.Value)) return;

            var spawnedPrefab = SpawnPrefab();
            spawnedPrefab.transform.position = Player.main.transform.position + Player.main.camRoot.transform.forward * Plugin.spawnDistance.Value;
            spawnedPrefab.transform.rotation = Quaternion.identity;
            spawnedPrefab.transform.localScale *= Plugin.spawnScaleMultiplier.Value;
        }

        private bool GetPrefab()
        {
            if(prefab) return true;

            var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.modFolderPath, "insert_bundle"));
            if(assetBundle == null)
            {
                Plugin.Logger.LogError("Failed to load asset bundle!");
                return false;
            }

            object[] assets = assetBundle.LoadAllAssets();
            prefab = (GameObject)assets.FirstOrDefault(i => i.ToString().Contains("test_model"));

            return prefab != null;
        }

        private GameObject SpawnPrefab()
        {
            var spawnedGO = Instantiate(prefab);
            
            MaterialUtils.ApplySNShaders(spawnedGO);

            return spawnedGO;
        }
    }
}
