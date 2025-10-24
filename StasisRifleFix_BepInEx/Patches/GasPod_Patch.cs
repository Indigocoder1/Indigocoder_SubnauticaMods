using HarmonyLib;

namespace StasisRifleFixMod_BepInEx;

[HarmonyPatch(typeof(GasPod))]
public class GasPod_Patch
{
    [HarmonyPatch(nameof(GasPod.Update)), HarmonyPrefix]
    private static bool Update_Prefix(GasPod __instance)
    {
        var rb = __instance.mainCollider.attachedRigidbody;
        if (!rb) return true;
        
        return !rb.isKinematic;
    }
}