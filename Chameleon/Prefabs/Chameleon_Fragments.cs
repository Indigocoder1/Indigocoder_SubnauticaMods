﻿using Chameleon.Monobehaviors;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chameleon.Prefabs
{
    internal static class Chameleon_Fragments
    {
        private const int EASTER_EGG_CHANCE = 10;

        public static PrefabInfo FragmentInfo { get; private set; } = PrefabInfo.WithTechType("ChameleonFragment", null, null, "English");
        private static GameObject[] fragmentGOs;
        private static Dictionary<GameObject, GameObject> cachedPrefabs = new();

        public static void Patch()
        {
            var prefab = new CustomPrefab(FragmentInfo);

            fragmentGOs = new GameObject[]
            {
                Main_Plugin.AssetBundle.LoadAsset<GameObject>("ChameleonFragment1"),
                Main_Plugin.AssetBundle.LoadAsset<GameObject>("ChameleonFragment2"),
                Main_Plugin.AssetBundle.LoadAsset<GameObject>("ChameleonFragment3")
            };

            prefab.RemoveFromCache();
            prefab.SetGameObject(GetFragmentGO);

            prefab.SetSpawns(new LootDistributionData.BiomeData[]
            {
                new() { biome = BiomeType.Dunes_TechSite_Scatter, count = 1, probability = 1f },
                new() { biome = BiomeType.Mountains_TechSite_Scatter, count = 1, probability = 1f },
            });

            PDAHandler.AddCustomScannerEntry(FragmentInfo.TechType, scanTime: 6f, destroyAfterScan: true);

            prefab.Register();
        }

        private static IEnumerator GetFragmentGO(IOut<GameObject> prefabOut)
        {
            GameObject prefab = fragmentGOs[Random.Range(0, fragmentGOs.Length)];

            if(cachedPrefabs.TryGetValue(prefab, out GameObject go))
            {
                prefabOut.Set(go);
                yield break;
            }

            prefab.SetActive(false);
            GameObject instance = GameObject.Instantiate(prefab);

            yield return new WaitUntil(() => MaterialUtils.IsReady);

            MaterialUtils.ApplySNShaders(instance);
            PrefabUtils.AddBasicComponents(instance, FragmentInfo.ClassID, FragmentInfo.TechType, LargeWorldEntity.CellLevel.Far);

            ChameleonFragment fragment = instance.GetComponent<ChameleonFragment>();
            fragment.SetEasterEggActive(Random.Range(0, 100) <= EASTER_EGG_CHANCE);

            cachedPrefabs.Add(prefab, instance);

            prefabOut.Set(instance);
        }
    }
}
