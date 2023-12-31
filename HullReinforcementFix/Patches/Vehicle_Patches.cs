using HarmonyLib;
using HullReinforcementFix.Craftables;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace HullReinforcementFix.Patches
{
    [HarmonyPatch(typeof(Vehicle))]
    internal static class Vehicle_Patches
    {
        [HarmonyPatch(nameof(Vehicle.OnUpgradeModuleChange)), HarmonyPostfix]
        private static void OnUpgradeModuleChange_Patch(Vehicle __instance, TechType techType, bool added)
        {
            if(techType != UpgradedHullReinfocement.mk2TechType && techType != UpgradedHullReinfocement.mk3TechType)
            {
                return;
            }

            if (!added)
            {
                return;
            }

            DealDamageOnImpact dealDamageOnImpact = __instance.GetComponent<DealDamageOnImpact>();
            int mk2Count = __instance.modules.GetCount(UpgradedHullReinfocement.mk2TechType);
            int mk3Count = __instance.modules.GetCount(UpgradedHullReinfocement.mk3TechType);
            float originalDamageFraction = dealDamageOnImpact.mirroredSelfDamageFraction;
            float mk2DamageFraction = mk2Count > 0 ? 0.4f * Mathf.Pow(0.55f, mk2Count) : 0;
            float mk3DamageFraction = mk3Count > 0 ? 0.3f * Mathf.Pow(0.3f, mk3Count) : 0;

            float newDamageFraction = originalDamageFraction;

            if ((mk2Count + mk3Count) != 0)
            {
                newDamageFraction = (mk2DamageFraction + mk3DamageFraction) / (mk2Count + mk3Count);
            }

            dealDamageOnImpact.mirroredSelfDamageFraction = newDamageFraction;

            if (Main_Plugin.WriteLogs.Value)
            {
                Main_Plugin.logger.LogInfo($"Original dmg fraction = {originalDamageFraction}");
                Main_Plugin.logger.LogInfo($"Altered damage fraction = {newDamageFraction}");
            }
        }
    }
}
