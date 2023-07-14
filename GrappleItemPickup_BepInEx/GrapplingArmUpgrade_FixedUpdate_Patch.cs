using HarmonyLib;
using GrapplingArmUpgrade_BepInEx;
using UnityEngine;
using BepInEx.Logging;

namespace GrappleItemPickup_BepInEx
{
    [HarmonyPatch(typeof(GrapplingArmUpgrade_Handler))]
    internal static class GrapplingArmUpgrade_FixedUpdate_Patch
    {
        private static bool WriteLogs = GrappleItemPickupPlugin.WriteLogs.Value;
        private static float PickupDistance = GrappleItemPickupPlugin.PickupDistance.Value;

        [HarmonyPatch(nameof(GrapplingArmUpgrade_Handler.FixedUpdate)), HarmonyPostfix]
        public static void GrapplingArm_Patch(GrapplingArmUpgrade_Handler __instance)
        {
            if (!GrappleItemPickupPlugin.EnableMod.Value)
            {
                return;
            }

            if (!__instance.hook.attached)
            {
                return;
            }

            if (WriteLogs)
                GrappleItemPickupPlugin.logger.Log(LogLevel.Info, "Attach flag passed");

            if (Vector2.Distance(__instance.front.position, __instance.hook.transform.position) > PickupDistance)
            {
                return;
            }

            if (WriteLogs)
                GrappleItemPickupPlugin.logger.Log(LogLevel.Info, "Distance flag passed, searching for item within radius");

            Collider[] colliders = Physics.OverlapSphere(__instance.hook.transform.position, 1f);

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.TryGetComponent<Pickupable>(out Pickupable pickupable);

                if (pickupable == null && colliders[i].transform.parent != null)
                {
                    pickupable = colliders[i].transform.gameObject.GetComponentInParent<Pickupable>();
                }

                if (pickupable == null)
                {
                    if (WriteLogs)
                        GrappleItemPickupPlugin.logger.Log(LogLevel.Info, $"Pickupable not found!");
                    continue;
                }

                if (WriteLogs)
                    GrappleItemPickupPlugin.logger.Log(LogLevel.Info, $"Pickupable = {pickupable}");

                if (pickupable)
                {
                    if (!__instance.exosuit.storageContainer.container.HasRoomFor(pickupable))
                    {
                        if (Player.main.GetVehicle() == __instance.exosuit)
                        {
                            ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                        }
                    }
                    else
                    {
                        //Thanks ExosuitDrillArm :)

                        string techName = Language.main.Get(pickupable.GetTechName());
                        ErrorMessage.AddMessage(Language.main.GetFormat<string>("VehicleAddedToStorage", techName));
                        uGUI_IconNotifier.main.Play(pickupable.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
                        pickupable.Initialize();
                        InventoryItem item = new InventoryItem(pickupable);
                        __instance.exosuit.storageContainer.container.UnsafeAdd(item);
                        pickupable.PlayPickupSound();

                        if (WriteLogs)
                            GrappleItemPickupPlugin.logger.Log(LogLevel.Debug, $"Adding item to container: {pickupable.name}");

                        __instance.ResetHook();
                        break;
                    }
                }
            }
        }
    }
}
