using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using UnityEngine;
using Discord;

namespace GrappleItemPickup
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    public static class GrappleItemPickup
    {
        [HarmonyPatch(nameof(ExosuitGrapplingArm.FixedUpdate)), HarmonyPostfix]
        public static void GrapplingArm_Patch(ExosuitGrapplingArm __instance)
        {
            if(!QMod.Config.enableMod)
            {
                return;
            }

            if (__instance.hook.staticAttached || !__instance.hook.attached)
            {
                return;
            }

            if (QMod.Config.writeLogs)
                Logger.Log(Logger.Level.Debug, "Attach flag passed");

            if (Vector2.Distance(__instance.front.position, __instance.hook.transform.position) > QMod.Config.pickupDistance)
            {
                return;
            }

            if (QMod.Config.writeLogs)
                Logger.Log(Logger.Level.Debug, "Distance flag passed");

            Collider[] colliders = Physics.OverlapSphere(__instance.hook.transform.position, QMod.Config.pickupDistance);

            if (QMod.Config.writeLogs)
                Logger.Log(Logger.Level.Debug, "Searching for item within radius");

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.TryGetComponent<Pickupable>(out Pickupable pickupable);

                if (pickupable == null && colliders[i].transform.parent != null)
                {
                    pickupable = colliders[i].transform.gameObject.GetComponentInParent<Pickupable>();
                }

                if (pickupable == null)
                {
                    if (QMod.Config.writeLogs)
                        Logger.Log(Logger.Level.Debug, $"Pickupable not found!");
                    continue;
                }

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

                        if (QMod.Config.writeLogs)
                            Logger.Log(Logger.Level.Debug, $"Adding item to container: {pickupable.name}");

                        __instance.ResetHook();
                        break;
                    }
                }
            }
        }
    }
}
