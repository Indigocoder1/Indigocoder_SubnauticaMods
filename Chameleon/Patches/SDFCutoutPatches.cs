using Chameleon.Monobehaviors;
using HarmonyLib;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(SDFCutout))]
    internal class SDFCutoutPatches
    {
        [HarmonyPatch(nameof(SDFCutout.Start), MethodType.Enumerator), HarmonyPrefix]
        private static bool Start_Prefix(SDFCutout __instance)
        {
            var objectInstance = (object)__instance;
            var fields = objectInstance.GetType().GetFields();

            SDFCutout cutout = (SDFCutout)fields[0].GetValue(objectInstance);
            var distanceFieldManager = cutout.GetComponent<DistanceFieldAssigner>();

            return distanceFieldManager == null;
        }
    }
}
