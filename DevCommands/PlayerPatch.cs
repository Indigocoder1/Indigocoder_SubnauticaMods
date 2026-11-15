using HarmonyLib;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DevCommands
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        private static Light light;
        private static bool visualizingBatches;
        private static Dictionary<Int3, GameObject> visualizers = new Dictionary<Int3, GameObject>();

        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Postfix(Player __instance)
        {
            light = Camera.main.gameObject.EnsureComponent<Light>();
            light.range = 100f;
            light.intensity = 1f;
            light.type = LightType.Spot;
            light.spotAngle = 100;
            light.enabled = false;
        }

        [HarmonyPatch(nameof(Player.Update)), HarmonyPostfix]
        private static void Update_Postfix()
        {
            foreach (var index in LargeWorldStreamer.main.loadedBatches)
            {
                if (visualizers.TryGetValue(index, out var cube))
                {
                    if ((cube.transform.position - Camera.main.transform.position).sqrMagnitude > 1000)
                    {
                        GameObject.Destroy(cube.gameObject);
                        visualizers.Remove(index);
                    }
                }
                else
                {
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject.Destroy(obj.GetComponent<Collider>());
                    obj.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Transparent");
                    int batchSize = LargeWorldStreamer.main.streamerV2.batchSize;
                    obj.transform.position = (Vector3)(index * batchSize + batchSize / 2);
                    obj.transform.localScale = Vector3.one * batchSize;
                    visualizers.Add(index, obj);
                }
            }
        }

        [HarmonyPatch(nameof(Player.OnDestroy)), HarmonyPostfix]
        private static void OnDestroy_Postfix()
        {
            visualizingBatches = false;
            visualizers.Clear();
        }

        public static void ToggleLightActive()
        {
            if (light == null) return;

            light.enabled = !light.enabled;
        }

        public static void ToggleBatchVisualization()
        {
            visualizingBatches = !visualizingBatches;
            if (!visualizingBatches)
            {
                foreach (var item in visualizers)
                {
                    GameObject.Destroy(item.Value);
                }

                visualizers.Clear();
            }
        }
    }
}
