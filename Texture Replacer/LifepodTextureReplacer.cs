using BepInEx;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UWE;
using static TextureReplacer.Main;

namespace TextureReplacer
{
    internal static class LifepodTextureReplacer
    {
        private static string folderFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer");
        private static string configFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "TextureReplacer/LifepodTextureConfig.json");

        private static List<LifepodConfigData> lifepodConfigs;

        public static void Initialize()
        {
            lifepodConfigs = SaveManager.LoadLifepodConfigFromJson(configFilePath);
            if (lifepodConfigs == null) 
            {
                SaveInitialData();
                lifepodConfigs = SaveManager.LoadLifepodConfigFromJson(configFilePath);
            }

            LoadAllTextures();
        }

        private static void LoadAllTextures()
        {
            for (int i = 0; i < lifepodConfigs.Count; i++)
            {
                LifepodConfigData configData = lifepodConfigs[i];
                if (configData == null)
                {
                    return;
                }

                CoroutineHost.StartCoroutine(InitializeTextures(configData));
            }
        }

        private static IEnumerator InitializeTextures(LifepodConfigData configData)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(LifepodClassIDs[(LifepodNumber)configData.lifepodIndex]);

            yield return request;

            if (request.TryGetPrefab(out GameObject prefab))
            {
                TextureReplacerHelper replacer = prefab.EnsureComponent<TextureReplacerHelper>();

                Renderer targetRenderer = prefab.transform.Find(ExternalRendererHierchyPaths[(LifepodNumber)configData.lifepodIndex]).GetComponent<Renderer>();

                if (targetRenderer == null)
                {
                    Main.logger.LogError($"Target renderer was null!");
                    yield break;
                }

                TexturePatchConfigData data = new TexturePatchConfigData($"Lifepod_Config{configData.lifepodIndex}_Index{configData.materialIndex}",
                    configData.materialIndex, configData.fileName, false, -1f, LifepodClassIDs[(LifepodNumber)configData.lifepodIndex],
                    ExternalRendererHierchyPaths[(LifepodNumber)configData.lifepodIndex], "_MainTex", 
                    new List<string> { $"Lifepod{configData.lifepodIndex}_Index{configData.materialIndex}" });
                replacer.AddTextureData(data);
            }
        }

        public static void SaveInitialData()
        {
            List<LifepodConfigData> lifepodConfigDatas = new List<LifepodConfigData>();

            for (int i = 0; i < ExternalRendererHierchyPaths.Count; i++)
            {
                int matIndex = 0;
                int matIndex2 = 1;

                string fileName1 = "life_pod_exterior_exploded_01_bloody.png";
                string fileName2 = "life_pod_exterior_exploded_02_bloody.png";

                LifepodNumber num = (LifepodNumber)i;
                string classID = LifepodClassIDs[num];
                string hierchy = ExternalRendererHierchyPaths[num];

                lifepodConfigDatas.Add(new LifepodConfigData(new Main.ConfigInfo(matIndex, fileName1, classID, hierchy, false, -1f,
                    new List<string> { $"Lifepod_Config{(int)num}_Index{matIndex2}" }), i, $"Lifepod_Config{(int)num}_Index{matIndex}"));
                lifepodConfigDatas.Add(new LifepodConfigData(new Main.ConfigInfo(matIndex2, fileName2, classID, hierchy, false, -1f,
                    new List<string> { $"Lifepod_Config{(int)num}_Index{matIndex}" }), i, $"Lifepod_Config{(int)num}_Index{matIndex2}"));
            }

            SaveManager.SaveLifepodConfigToJson(lifepodConfigDatas, configFilePath, folderFilePath);
        }

        private static readonly Dictionary<LifepodNumber, string> ExternalRendererHierchyPaths = new Dictionary<LifepodNumber, string>
        {
            { LifepodNumber.Lifepod2, "life_pod_exploded_02/life_pod"},
            { LifepodNumber.Lifepod3, "life_pod_exploded_02_01/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod4, "life_pod_exploded_01/life_pod"},
            { LifepodNumber.Lifepod6, "life_pod_exploded_02_02/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod7, "life_pod_exploded_02/life_pod"},
            { LifepodNumber.Lifepod12, "life_pod_exploded_02_03/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod13, "life_pod_exploded_02_02/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod17, "life_pod_exploded_02_03/exterior/life_pod_damaged"},
            { LifepodNumber.Lifepod19, "life_pod_exploded_02_01/exterior/life_pod_damaged"},
        };
        private static readonly Dictionary<LifepodNumber, string> LifepodClassIDs = new Dictionary<LifepodNumber, string>
        {
            { LifepodNumber.Lifepod2, "66cc5a83-142b-4d8d-8d16-2d6e960f59c3"},
            { LifepodNumber.Lifepod3, "2aa237f6-2103-4a78-aaa7-104216551f0a"},
            { LifepodNumber.Lifepod4, "f2b9fe45-39d6-4307-b1e0-143eb1937d6e"},
            { LifepodNumber.Lifepod6, "85ae70e0-176c-4de6-8c4d-48c4f504cc79"},
            { LifepodNumber.Lifepod7, "d3b9095f-fcac-46de-83f7-762e3275e837"},
            { LifepodNumber.Lifepod12, "00891fdf-7264-4c55-b569-732cdcded701"},
            { LifepodNumber.Lifepod13, "00037e80-3037-48cf-b769-dc97c761e5f6"},
            { LifepodNumber.Lifepod17, "56b5ed17-2bff-4f7e-aba0-275b6a2398f9"},
            { LifepodNumber.Lifepod19, "3894aeaf-e1f9-426a-9249-6a4968ac2d8b"},
        };

        public class LifepodConfigData
        {
            public int lifepodIndex;
            public string configName;
            public int materialIndex;
            public string fileName;

            public bool isVariation;
            public float variationChance;
            public List<string> linkedConfigNames;
            [JsonIgnore]
            public bool variationAccepted;

            [JsonConstructor]
            public LifepodConfigData(int lifepodIndex, string configName, int materialIndex, string fileName, bool isVariation, float variationChance, List<string> linkedConfigNames)
            {
                this.lifepodIndex = lifepodIndex;
                this.configName = configName;
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
                this.linkedConfigNames = linkedConfigNames;
            }

            public LifepodConfigData(ConfigInfo configInfo, int lifepodIndex, string configName)
            {
                this.lifepodIndex = lifepodIndex;
                this.configName = configName;
                this.materialIndex = configInfo.materialIndex;
                this.fileName = configInfo.fileName;
                this.isVariation = configInfo.isVariation;
                this.variationChance = configInfo.variationChance;
                this.linkedConfigNames = configInfo.linkedConfigNames;
            }
        }

        public enum LifepodNumber
        {
            Lifepod2,
            Lifepod3,
            Lifepod4,
            Lifepod6,
            Lifepod7,
            Lifepod12,
            Lifepod13,
            Lifepod17,
            Lifepod19
        }
    }
}
