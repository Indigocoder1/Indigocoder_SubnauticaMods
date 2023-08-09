using HarmonyLib;

namespace UpgradedJumpJetModule
{
    [HarmonyPatch(typeof(Exosuit))]
    internal class Exosuit_Update_Patch
    {
        [HarmonyPatch(nameof(Exosuit.Update)), HarmonyPrefix]
        private static void Patch(Exosuit __instance)
        {
            float defaultThrustConsumption = 0.09f;
            float aboveWaterThrustConsumption = 0.35f;

            if(__instance.modules.GetCount(UpgradedJetsModule.techType) == 0)
            {
                return;
            }

            bool underwater = __instance.transform.position.y < __instance.worldForces.waterDepth + 2f && !__instance.precursorOutOfWater;

            if (!underwater)
            {
                if(__instance.jetsActive)
                {
                    __instance.thrustConsumption = aboveWaterThrustConsumption;
                }
                else
                {
                    __instance.thrustConsumption = defaultThrustConsumption;
                }
            }
            else
            {
                __instance.thrustConsumption = defaultThrustConsumption;
            }
        }
    }
}
