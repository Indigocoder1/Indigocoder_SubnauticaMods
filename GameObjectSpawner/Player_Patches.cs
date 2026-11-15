using HarmonyLib;

namespace GameObjectSpawner
{
    [HarmonyPatch(typeof(Player))]
    internal class Player_Patches
    {
        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Postfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<SpawnerMonobehavior>();
        }
    }
}
