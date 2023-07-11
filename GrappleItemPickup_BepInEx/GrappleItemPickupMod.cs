using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;

namespace GrappleItemPickup_BepInEx
{
    [HarmonyPatch(typeof(Exosuit))]
    public static class GrappleItemPickupMod
    {
        private static bool WriteLogs = GrappleItemPickupModOptions.WriteLogs.Value;
        private static float PickupDistance = GrappleItemPickupModOptions.PickupDistance.Value;

        [HarmonyPatch(nameof(Exosuit.Update)), HarmonyPostfix]
        public static void ExosuitPatch(Exosuit __instance)
        {
            Exosuit instance = __instance;

            if (instance.leftArm.GetType() == typeof(ExosuitGrapplingArm))
            {
                ArmLogic((ExosuitGrapplingArm)instance.leftArm);
            }

            if (instance.rightArm.GetType() == typeof(ExosuitGrapplingArm))
            {
                ArmLogic((ExosuitGrapplingArm)instance.rightArm);
            }
        }

        public static void ArmLogic(ExosuitGrapplingArm arm)
        {
            if (arm.hook.staticAttached || !arm.hook.attached)
            {
                return;
            }

            if (WriteLogs)
                GrappleItemPickupPlugin.logger.Log(LogLevel.Info, "Attach flag passed");

            if (Vector2.Distance(arm.front.position, arm.hook.transform.position) > PickupDistance)
            {
                return;
            }

            if (WriteLogs)
                GrappleItemPickupPlugin.logger.Log(LogLevel.Info, "Distance flag passed");

            Collider[] colliders = Physics.OverlapSphere(arm.hook.transform.position, PickupDistance);

            if (WriteLogs)
                GrappleItemPickupPlugin.logger.Log(LogLevel.Debug, "Searching for item within radius");

            for (int i = 0; i < colliders.Length; i++)
            {
                Pickupable pickupable = colliders[i].gameObject.GetComponentInChildren<Pickupable>();

                if (pickupable == null)
                {
                    pickupable = colliders[i].transform.parent.gameObject.GetComponentInChildren<Pickupable>();
                }

                if (pickupable)
                {
                    if (!arm.exosuit.storageContainer.container.HasRoomFor(pickupable))
                    {
                        if (Player.main.GetVehicle() == arm.exosuit)
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
                        arm.exosuit.storageContainer.container.UnsafeAdd(item);
                        pickupable.PlayPickupSound();

                        if (WriteLogs)
                            GrappleItemPickupPlugin.logger.Log(LogLevel.Debug, $"Adding item to container: {pickupable.name}");

                        arm.ResetHook();
                        break;
                    }
                }
            }
        }
    }
}
