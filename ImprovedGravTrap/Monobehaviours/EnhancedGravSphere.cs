using ImprovedGravTrap.Monobehaviours;
using IndigocoderLib;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImprovedGravTrap
{
    internal class EnhancedGravSphere : MonoBehaviour
    {
        public StorageContainer storageContainer { get; set; }
        public List<Rigidbody> bufferedAttractables = new();
        public PingInstance pingInstance { get; set; }

        private Pickupable pickupable;
        private Gravsphere gravSphere;
        private GameObject model;
        private Animator animator;
        private bool inRangeActivated;
        private bool resetTriggers;
        private bool targetingStorage;
        private bool targetStorageHasSpace;
        private string targetStorageName;

        private void Start() 
        {
            pickupable = GetComponent<Pickupable>();
            pingInstance = GetComponent<PingInstance>();
            gravSphere = GetComponent<Gravsphere>();
            animator = GetComponentInChildren<Animator>();

            model = transform.Find("gravSphere_anim").gameObject;

            pingInstance.SetLabel("Enhanced Grav Trap");

            storageContainer = GetComponentInChildren<StorageContainer>();
            Player.main.pda.onCloseCallback += (pda) =>
            {
                storageContainer.OnClosePDA(pda);

            };

            UpdateRange();
        }

        private void Update()
        {
            pingInstance.visible = !pickupable.attached;

            HandleStorageDeposition();
            HandleStorageAdding();
            HandleBufferTransfer();
            HandleHeldEnable();
            HandleStorageOpening();
            HandleModeAdvancing();
            HandleHandText();

            if (!gravSphere.pickupable.isValidHandTarget) return;

            HandleFarDistancePopIn();
        }

        private void HandleStorageAdding()
        {
            for (int i = 0; i < gravSphere.attractableList.Count; i++)
            {
                Rigidbody rb = gravSphere.attractableList[i];
                if (!rb)
                {
                    continue;
                }

                if (Vector3.Distance(rb.position, gravSphere.transform.position) > Main_Plugin.GravStoragePickupDistance.Value)
                {
                    continue;
                }

                if (rb.TryGetComponent(out BreakableResource resource))
                {
                    resource.BreakIntoResources();
                }

                if (rb.TryGetComponent(out Pickupable pickupable))
                {
                    if (!storageContainer.container.HasRoomFor(pickupable))
                    {
                        continue;
                    }

                    pickupable.Initialize();
                    storageContainer.container.AddItem(pickupable);

                    int lastIndex = bufferedAttractables.Count - 1;
                    if (lastIndex <= 0)
                    {
                        return;
                    }

                    Rigidbody bufferedRb = bufferedAttractables[lastIndex];
                    if (!bufferedRb.isKinematic)
                    {
                        gravSphere.AddAttractable(bufferedRb);
                        bufferedAttractables.Remove(bufferedRb);
                    }
                    UWE.Utils.SetIsKinematicAndUpdateInterpolation(bufferedRb, false, false);
                }
            }
        }
        private void HandleBufferTransfer()
        {
            for (int i = bufferedAttractables.Count - 1; i >= 0; i--)
            {
                Rigidbody rb = bufferedAttractables[i];
                if (!rb)
                {
                    bufferedAttractables.RemoveAt(i);
                    continue;
                }
                Pickupable pickupable = rb.GetComponent<Pickupable>();
                if (!pickupable)
                {
                    continue;
                }

                if (storageContainer.container.HasRoomFor(pickupable))
                {
                    gravSphere.AddAttractable(rb);
                    bufferedAttractables.RemoveAt(i);
                }
            }
        }
        private void HandleHeldEnable()
        {
            if (!Player.main.IsInside())
            {
                if (GameInput.GetButtonDown(GameInput.Button.LeftHand) && Inventory.main.quickSlots.heldItem?.item == gravSphere.pickupable)
                {
                    if (!gravSphere.trigger.enabled && !resetTriggers)
                    {
                        gravSphere.trigger.enabled = true;
                        gravSphere.gameObject.GetComponent<Pickupable>().attached = false;
                    }
                    else
                    {
                        gravSphere.DeactivatePads();
                        gravSphere.trigger.enabled = false;
                        resetTriggers = false;
                        gravSphere.gameObject.GetComponent<Pickupable>().attached = true;
                    }
                }
            }
            else if (gravSphere.trigger.enabled)
            {
                if (!HoldingItem()) return;

                gravSphere.DeactivatePads();
                gravSphere.trigger.enabled = false;
                gravSphere.gameObject.GetComponent<Pickupable>().attached = true;
            }
        }
        private void HandleStorageOpening()
        {
            if (targetingStorage) return;

            //If deployed, in range and open key pressed: open storage
            if (PlayerInRange() && GameInput.GetButtonDown(GameInput.Button.AltTool) && !Player.main.pda.isOpen)
            {
                storageContainer.Open();
            }
        }
        private void HandleModeAdvancing()
        {
            if (!HoldingItem() && !PlayerInRange()) return;

            GravTrapObjectsType objectsType = gravSphere.GetComponent<GravTrapObjectsType>();

            int indexDelta = GameInput.GetButtonDown(GameInput.Button.Deconstruct) ? 1 : 0;
            int nextIndex = (objectsType.techTypeListIndex + indexDelta) % Main_Plugin.AllowedTypes.Count;

            objectsType.techTypeListIndex = nextIndex;
        }
        private void HandleHandText()
        {
            if (!HoldingItem()) return;

            GravTrapObjectsType objectsType = gravSphere.GetComponent<GravTrapObjectsType>();

            string primaryString =
                $"{Language.main.GetFormat("HandReticleAddButtonFormat", "Deploy gravtrap", uGUI.FormatButton(GameInput.Button.RightHand))}"; 

            string gravtrapactivate =
                    storageContainer.container.IsFull() ? "Cannot activate; storage is full" :
                    !gravSphere.trigger.enabled && !resetTriggers ? "Activate gravtrap" :
                    "Deactivate gravtrap";

            string storageText = "Open storage";
            if (targetingStorage)
            {
                storageText = targetStorageHasSpace ? $"Deposit storage ({targetStorageName})" : "Target storage full";
            }

            string secondaryString =
                $"{Language.main.GetFormat("HandReticleAddButtonFormat", gravtrapactivate, uGUI.FormatButton(GameInput.Button.LeftHand))}\n" +
                $"{Language.main.GetFormat("HandReticleAddButtonFormat", storageText, uGUI.FormatButton(GameInput.Button.AltTool))} | " +
                $"{Language.main.GetFormat("HandReticleAddButtonFormat", $"Advance mode ({objectsType.GetCurrentListName()})",
                uGUI.FormatButton(GameInput.Button.Deconstruct))}";

            if (!storageContainer.GetOpen() && !IngameMenu.main.selected)
            {
                HandReticle.main.textUse = primaryString;
                HandReticle.main.textUseSubscript = secondaryString;
            }
        }
        private void HandleFarDistancePopIn()
        {
            if ((transform.position - Player.main.transform.position).sqrMagnitude > 10000 && gameObject.activeSelf)
            {
                inRangeActivated = false;

                gravSphere.enabled = false;
                model.SetActive(false);
            }
            else if (!inRangeActivated)
            {
                inRangeActivated = true;

                gravSphere.enabled = true;
                gravSphere.trigger.enabled = true;
                gravSphere.pickupable.attached = false;

                model.SetActive(true);
                animator.SetBool("deployed", true);
            }
        }
        private void HandleStorageDeposition()
        {
            targetingStorage = false;
            targetStorageHasSpace = false;

            if (!HoldingItem()) return;

            Transform mainCamTransform = MainCamera.camera.transform;
            Vector3 camForward = mainCamTransform.forward;
            if (!Physics.Raycast(mainCamTransform.position + (camForward * 0.15f), camForward, out var raycastHit, 100f)) return;

            if(raycastHit.collider == null) return;

            GameObject target = raycastHit.collider.gameObject;

            GameObject rootTarget = UWE.Utils.GetEntityRoot(target) ?? target;

            var targetTechType = CraftData.GetTechType(rootTarget);

            if (targetTechType == TechType.Player) return;

            string trimmedName = Utilities.GetNameWithCloneRemoved(rootTarget.name);
            targetStorageName = targetTechType == TechType.None ? trimmedName : Language.main.Get(targetTechType);

            var storageContainers = rootTarget.GetComponentsInChildren<StorageContainer>(true).Where(container =>
            {
                foreach (ItemsContainer.ItemGroup itemGroup in storageContainer.container._items.Values)
                {
                    InventoryItem item = itemGroup.items.Count > 0 ? itemGroup.items[0] : null;
                    if (item == null) continue;

                    TechType techType = item.item.overrideTechUsed ? item.item.overrideTechType : item.item.GetTechType();
                    if (container.container.IsTechTypeAllowed(techType))
                    {
                        return true;
                    }
                }

                return false;
            }).ToList();

            ItemsContainer targetContainer = (from container in storageContainers
                where !container.container.IsFull()
                select container.container).FirstOrDefault();

            if(storageContainers.Any())
            {
                targetingStorage = true;

                if(targetContainer != null && !targetContainer.IsFull())
                {
                    targetStorageHasSpace = true;
                }
            }
            else if (rootTarget.TryGetComponent(out Vehicle targetVehicle))
            {
                switch (targetVehicle)
                {
                    case Exosuit exosuit:

                        var container = exosuit.storageContainer.container;
                        targetingStorage = true;
                        if(!container.IsFull())
                        {
                            targetStorageHasSpace = true;
                            targetContainer = container;
                        }

                        break;
                    case SeaMoth seaMoth:
                        for (int i = 0; i < 12; i++)
                        {
                            try
                            {
                                var storage = seaMoth.GetStorageInSlot(i, TechType.VehicleStorageModule);
                                if (storage == null) continue;
                                targetingStorage = true;
                                if (storage.IsFull()) continue;
                                targetStorageHasSpace = true;
                                targetContainer = storage;
                            }
                            catch
                            {
                                //Ignore
                            }
                        }
                        break;
                }
            }
            else
            {
                var escapePod = target.GetComponentInParent<EscapePod>();
                var handTarget = target.GetComponent<GenericHandTarget>();
                if (escapePod && handTarget)
                {
                    var container = escapePod.storageContainer.container;
                    targetingStorage = true;
                    if (!container.IsFull())
                    {
                        targetStorageHasSpace = true;
                        targetContainer = container;
                    }
                }
            }

            if (targetContainer == null) return;
            if (!GameInput.GetButtonDown(GameInput.Button.AltTool)) return;

            Main_Plugin.logger.LogInfo($"Target container = {targetContainer.tr.name}");

            foreach (var itemGroup in new List<ItemsContainer.ItemGroup>(storageContainer.container._items.Values))
            {
                foreach (InventoryItem inventoryItem in new List<InventoryItem>(itemGroup.items))
                {
                    TechType techType = inventoryItem.item.overrideTechUsed ? inventoryItem.item.overrideTechType : inventoryItem.item.GetTechType();
                    if(targetContainer.IsTechTypeAllowed(techType) && ((IItemsContainer)targetContainer).AddItem(inventoryItem))
                    {
                        ErrorMessage.AddMessage($"Moved {inventoryItem.item.GetTechName()} to {targetStorageName}");
                        continue;
                    }

                    if (storageContainers.Count <= 0) continue;

                    var newContainer = storageContainers.Where(container =>
                    {
                        if (container.container.IsFull()) return false;
                        if (!container.container.IsTechTypeAllowed(techType)) return false;
                        if (!((IItemsContainer)container.container).AddItem(inventoryItem)) return false;

                        return true;
                    }).Select(container => container.container).FirstOrDefault();

                    if(newContainer != null)
                    {
                        targetContainer = newContainer;
                    }
                }
            }
        }

        private void UpdateRange()
        {
            SphereCollider[] sphereColliders = gameObject.GetComponents<SphereCollider>();

            if (sphereColliders?.FirstOrDefault(s => s.radius >= 10) is SphereCollider sphere)
            {
                sphere.radius = Main_Plugin.EnhancedRange.Value;
            }
        }

        private bool HoldingItem()
        {
            if (Inventory.main.quickSlots.heldItem == null) return false;

            if (!Inventory.main.quickSlots.heldItem.techType.IsEnhancedGravTrap()) return false;

            if (!Inventory.main.quickSlots.heldItem.item == gravSphere.pickupable) return false;

            return true;
        }

        private bool PlayerInRange()
        {
            return Vector3.Distance(gravSphere.transform.position, Player.main.transform.position) <= Main_Plugin.GravStorageOpenDistance.Value;
        }
    }
}
