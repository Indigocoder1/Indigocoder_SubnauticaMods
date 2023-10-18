using HarmonyLib;
using UnityEngine;

namespace MirrorMod.Monobehaviors
{
    [HarmonyPatch(typeof(Player))]
    internal static class Mirror_Manager
    {
        public static Plane[] mainCameraFrustumPlanes;
        private static Camera mainCamera;

        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Player_Start()
        {
            mainCamera = Camera.main;
        }

        [HarmonyPatch(nameof(Player.Update)), HarmonyPostfix]
        private static void Player_Update()
        {
            mainCameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        }
    }
}
