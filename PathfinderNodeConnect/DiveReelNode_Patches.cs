using HarmonyLib;

namespace PathfinderNodeConnect
{
    [HarmonyPatch(typeof(DiveReelNode))]
    internal static class DiveReelNode_Patches
    {
        [HarmonyPatch(nameof(DiveReelNode.Update)), HarmonyPostfix]
        private static void Update_Patch(DiveReelNode __instance)
        {
            if(__instance.rb.velocity.sqrMagnitude <= 0.5f && !DiveReel_Patches.checkedStoppedNodes.Contains(__instance))
            {
                DiveReel_Patches.nodesStoppedMoving.Add(__instance);
            }
        }
    }
}
