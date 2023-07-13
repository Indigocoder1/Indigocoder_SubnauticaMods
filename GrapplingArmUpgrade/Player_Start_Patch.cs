using HarmonyLib;
using UnityEngine;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(Player))]
    internal static class Player_Start_Patch
    {
        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Patch()
        {
            GrapplingArmUpgradeModule.RegisterModule();
        }
    }
}
