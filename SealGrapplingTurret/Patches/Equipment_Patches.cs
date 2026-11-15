using BepInEx;
using HarmonyLib;
using SealGrapplingTurret.Items;
using SealSubMod.MonoBehaviours;
using UnityEngine;
using static VFXParticlesPool;

namespace SealGrapplingTurret.Patches
{
    [HarmonyPatch(typeof(Equipment))]
    internal static class Equipment_Patches
    {
        [HarmonyPatch(nameof(Equipment.NotifyEquip)), HarmonyPostfix]
        private static void NotifyEquip(Equipment __instance, InventoryItem item)
        {
            if (IsSealConsole(__instance, item, out Transform modelParent))
            {
                Main_Plugin.logger.LogInfo("Is seal console!");
                GameObject baseGO = modelParent.Find("Cylinder.003").gameObject;
                GameObject turretGO = modelParent.Find("Cylinder.005").gameObject;
                GameObject barrelGO = modelParent.Find("Cylinder.006").gameObject;
                baseGO.SetActive(true);
                turretGO.SetActive(true);
                barrelGO.SetActive(true);
            }
        }

        [HarmonyPatch(nameof(Equipment.NotifyUnequip)), HarmonyPostfix]
        private static void NotifyUnequip(Equipment __instance, InventoryItem item)
        {
            if (IsSealConsole(__instance, item, out Transform modelParent))
            {
                Main_Plugin.logger.LogInfo("Is seal console!");
                GameObject baseGO = modelParent.Find("Cylinder.003").gameObject;
                GameObject turretGO = modelParent.Find("Cylinder.005").gameObject;
                GameObject barrelGO = modelParent.Find("Cylinder.006").gameObject;
                baseGO.SetActive(false);
                turretGO.SetActive(false);
                barrelGO.SetActive(false);
            }
        }

        private static bool IsSealConsole(Equipment __instance, InventoryItem item, out Transform modelParent)
        {
            modelParent = null;

            if (item.techType != GrappleModule.techType)
            {
                Main_Plugin.logger.LogInfo("Incorrect tech type");
                return false;
            }

            SealUpgradeConsole console = __instance.owner.GetComponent<SealUpgradeConsole>();
            if (!console)
            {
                Main_Plugin.logger.LogInfo("No console");
                return false;
            }

            if (console.transform.parent.TryGetComponent<SubRoot>(out SubRoot subRoot))
            {
                modelParent = subRoot.transform.Find("Scaler/SealSubModelPrefab/Seal Sub");
                return true;
            }

            Main_Plugin.logger.LogInfo("No sub root");
            return false;
        }
    }
}
