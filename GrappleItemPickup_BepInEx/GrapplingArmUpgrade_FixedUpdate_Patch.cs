using HarmonyLib;
using UnityEngine;

namespace GrappleItemPickup_BepInEx
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    internal static class GrapplingArmUpgrade_FixedUpdate_Patch
    {
        [HarmonyPatch(nameof(ExosuitGrapplingArm.FixedUpdate)), HarmonyPostfix]
        public static void GrapplingArm_Patch(ExosuitGrapplingArm __instance)
        {
            if (!GrappleItemPickupPlugin.EnableMod.Value)
            {
                return;
            }

            if (!__instance.hook.attached)
            {
                return;
            }

            if (GrappleItemPickupPlugin.WriteLogs.Value)
                GrappleItemPickupPlugin.logger.LogInfo("Attach flag passed");

            if (Vector2.Distance(__instance.front.position, __instance.hook.transform.position) > GrappleItemPickupPlugin.PickupDistance.Value)
            {
                return;
            }

            if (GrappleItemPickupPlugin.WriteLogs.Value)
                GrappleItemPickupPlugin.logger.LogInfo("Distance flag passed; searching for item within radius");

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
                    if (GrappleItemPickupPlugin.WriteLogs.Value)
                        GrappleItemPickupPlugin.logger.LogInfo($"Pickupable not found!");
                    continue;
                }

                if (GrappleItemPickupPlugin.WriteLogs.Value)
                    GrappleItemPickupPlugin.logger.LogInfo($"Pickupable = {pickupable}");

                GrappleItemPickupPlugin.logger.LogInfo($"Pickupable (bool) = {(bool)pickupable}");

                if (!pickupable)
                {
                    return;
                }

                Vehicle pickupableVehicle = pickupable.GetComponent<Vehicle>();
                if (pickupableVehicle == Player.main.GetVehicle())
                {
                    if (GrappleItemPickupPlugin.WriteLogs.Value)
                        GrappleItemPickupPlugin.logger.LogInfo("Returning because the item trying to be picked up is the current Vehicle");

                    return;
                }

                SubRoot pickupableSubRoot = pickupable.GetComponent<SubRoot>();
                if (pickupableSubRoot == Player.main.GetCurrentSub())
                {
                    if (GrappleItemPickupPlugin.WriteLogs.Value)
                        GrappleItemPickupPlugin.logger.LogInfo("Returning because the item trying to be picked up is the current SubRoot");

                    return;
                }

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

                    if (GrappleItemPickupPlugin.WriteLogs.Value)
                        GrappleItemPickupPlugin.logger.LogInfo($"Adding {pickupable.name} to prawn storage");

                    __instance.ResetHook();
                    break;
                }
            }
        }
    }
}
