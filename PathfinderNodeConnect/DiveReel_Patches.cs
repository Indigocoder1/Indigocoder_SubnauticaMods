using HarmonyLib;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace PathfinderNodeConnect
{
    [HarmonyPatch(typeof(DiveReel))]
    internal static class DiveReel_Patches
    {
        private static Dictionary<DiveReel, LineRenderer> lineRenderers = new Dictionary<DiveReel, LineRenderer>();
        public static List<DiveReelNode> nodesStoppedMoving = new List<DiveReelNode>();
        public static List<DiveReelNode> checkedStoppedNodes { get; private set; } = new List<DiveReelNode>();

        [HarmonyPatch(nameof(DiveReel.Start)), HarmonyPostfix]
        private static void Start_Patch(DiveReel __instance)
        {
            LineRenderer lr = GameObject.Instantiate(new GameObject()).EnsureComponent<LineRenderer>();
            lr.gameObject.name = "Pathfinder tool line renderer";
            lr.material = Main_Plugin.assetBundle.LoadAsset<Material>("LrMat");
            lr.startWidth = 0.4f;
            lr.endWidth = 0.4f;
            lr.textureMode = LineTextureMode.Tile;
            lr.numCornerVertices = 10;

            lineRenderers.Add(__instance, lr);
        }

        [HarmonyPatch(nameof(DiveReel.Update)), HarmonyPostfix]
        private static void Update_Patch(DiveReel __instance)
        {
            bool nodeStopped = false;
            DiveReelNode node = null;
            for (int i = nodesStoppedMoving.Count - 1; i >= 0; i--)
            {
                if (__instance.nodes.Contains(nodesStoppedMoving[i]))
                {
                    nodeStopped = true;
                    node = nodesStoppedMoving[i];
                    break;
                }
            }

            if(nodeStopped)
            {
                nodesStoppedMoving.Remove(node);
                checkedStoppedNodes.Add(node);
                return;
            }

            LineRenderer lr = lineRenderers[__instance];
            lr.positionCount = __instance.nodes.Count;
            for (int i = 0; i < __instance.nodes.Count; i++)
            {
                Vector3 position = __instance.nodes[i].transform.position;
                lr.SetPosition(i, position);
            }

            lineRenderers[__instance] = lr;
        }

        [HarmonyPatch(nameof(DiveReel.ResetNodes)), HarmonyPostfix]
        private static void ResetNodes_Patch()
        {
            nodesStoppedMoving.Clear();
            checkedStoppedNodes.Clear();
        }
    }
}
