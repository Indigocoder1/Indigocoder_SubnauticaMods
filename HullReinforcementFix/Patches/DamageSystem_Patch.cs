using HarmonyLib;
using HullReinforcementFix.Craftables;
using UnityEngine;
using static VFXParticlesPool;

namespace HullReinforcementFix
{
    [HarmonyPatch(typeof(DamageSystem))]
    internal static class DamageSystem_Patch
    {
        [HarmonyPatch(nameof(DamageSystem.CalculateDamage)), HarmonyPostfix]
        private static void CalculateDamage_Patch(GameObject target, DamageType type, ref float __result)
        {
            if(type == DamageType.Collide)
            {
                return;
            }

            LiveMixin mixin = target.GetComponent<LiveMixin>();
            if(mixin == null)
            {
                return;
            }

            float damage = __result;

            if (mixin.TryGetComponent<Vehicle>(out Vehicle vehicle))
            {
                int mk1HullModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);
                int mk2HullModuleCount = vehicle.modules.GetCount(UpgradedHullReinfocement.mk2TechType);
                int mk3HullModuleCount = vehicle.modules.GetCount(UpgradedHullReinfocement.mk3TechType);
                float mk1Damage = Mathf.Max(0, 1f / Main_Plugin.MK1DamageReductionMultiplier.Value * Mathf.Pow(0.9f, mk1HullModuleCount) - 0.2f);
                float mk2Damage = Mathf.Max(0, 1f / Main_Plugin.MK2DamageReductionMultiplier.Value * Mathf.Pow(0.85f, mk2HullModuleCount) - 0.2f);
                float mk3Damage = Mathf.Max(0, 1f / Main_Plugin.MK3DamageReductionMultiplier.Value * Mathf.Pow(0.8f, mk3HullModuleCount) - 0.2f);

                mk1Damage = mk1HullModuleCount == 0 ? 1 : mk1Damage;
                mk2Damage = mk2HullModuleCount == 0 ? 1 : mk2Damage;
                mk3Damage = mk3HullModuleCount == 0 ? 1 : mk3Damage;

                float reducedDamage = 0;
                if ((mk1HullModuleCount + mk2HullModuleCount + mk3HullModuleCount) != 0)
                {
                    reducedDamage = damage * ((mk1Damage + mk2Damage + mk3Damage) / (mk1HullModuleCount + mk2HullModuleCount + mk3HullModuleCount));
                }
                else
                {
                    reducedDamage = __result;
                }

                damage = Mathf.Max(reducedDamage, 0);

                if (Main_Plugin.WriteLogs.Value)
                {
                    Main_Plugin.logger.LogInfo($"Original dmg = {__result} (Mk1 Module Count = {mk1HullModuleCount} | " +
                        $"Mk2 Module Count = {mk2HullModuleCount} | Mk3 Module Count = {mk3HullModuleCount}");
                    Main_Plugin.logger.LogInfo($"Altered damages: {reducedDamage} | {mk1Damage} | {mk2Damage} | {mk3Damage}");
                }
            }

            __result = damage;
        }
    }
}
