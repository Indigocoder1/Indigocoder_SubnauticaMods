using HarmonyLib;
using UnityEngine;
using static VFXParticlesPool;

namespace HullReinforcementFix
{
    [HarmonyPatch(typeof(DamageSystem))]
    internal static class DamageSystem_Patch
    {
        [HarmonyPatch(nameof(DamageSystem.CalculateDamage)), HarmonyPostfix]
        private static void CalculateDamage_Patch(GameObject target, ref float __result)
        {
            LiveMixin mixin = target.GetComponent<LiveMixin>();
            if(mixin == null)
            {
                return;
            }

            float damage = __result;
            if (mixin.TryGetComponent<Vehicle>(out Vehicle vehicle))
            {
                int hullModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);
                damage -= hullModuleCount * Main_Plugin.DamageReductionMultiplier.Value;
                damage = Mathf.Max(damage, 0);

                if (Main_Plugin.WriteLogs.Value)
                {
                    Main_Plugin.logger.LogInfo($"Original dmg = {__result}");
                    Main_Plugin.logger.LogInfo($"Altered dmg = {damage}");
                }
            }

            __result = damage;
        }
    }
}
