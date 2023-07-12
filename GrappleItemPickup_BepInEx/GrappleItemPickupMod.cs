using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;

namespace GrappleItemPickup_BepInEx
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    public static class GrappleItemPickupMod
    {
        private static bool WriteLogs = GrappleItemPickupPlugin.WriteLogs.Value;
        private static float PickupDistance = GrappleItemPickupPlugin.PickupDistance.Value;

        [HarmonyPatch(nameof(ExosuitGrapplingArm.FixedUpdate)), HarmonyPostfix]
        public static void GrapplingArm_Patch(ExosuitGrapplingArm __instance)
        {
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

            Collider[] colliders = Physics.OverlapSphere(__instance.hook.transform.position, PickupDistance * 2f);

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.TryGetComponent<Pickupable>(out Pickupable pickupable);

                if (pickupable == null && colliders[i].transform.parent != null)
                {
                    pickupable = colliders[i].transform.gameObject.GetComponentInParent<Pickupable>();
                }

                if (pickupable == null)
                {
                    if(WriteLogs)
                        GrappleItemPickupPlugin.logger.Log(LogLevel.Info, $"Pickupable not found! | Collier[i].root = {colliders[i].transform.root}");
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
