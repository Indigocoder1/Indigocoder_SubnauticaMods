using HarmonyLib;
using System.Net;
using UnityEngine;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(WarpBall))]
    internal static class WarpBall_Patch
    {
        [HarmonyPatch(nameof(WarpBall.Warp)), HarmonyPrefix]
        private static bool Patch(WarpBall __instance, GameObject target)
        {
            Player component = target.GetComponent<Player>();

            if(component == null)
            {
                return true;
            }

            bool hasSuit = Inventory.main.equipment.GetCount(Suit_Craftable.suitTechType) > 0;
            bool hasGloves = Inventory.main.equipment.GetCount(Gloves_Craftable.glovesTechType) > 0;

            if (hasSuit && hasGloves)
            {
                GameObject.Destroy(__instance.gameObject);
                return false;
            }

            return true;
        }
    }
}
