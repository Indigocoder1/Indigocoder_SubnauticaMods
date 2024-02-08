using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using Nautilus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static VFXParticlesPool;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(Gravsphere))]
    internal static class GravTrap_Patches
    {
        private static readonly Dictionary<Gravsphere, List<Rigidbody>> bufferedAttractables = new Dictionary<Gravsphere, List<Rigidbody>>();
        private static Dictionary<Gravsphere, bool> ResetTriggers = new Dictionary<Gravsphere, bool>();

        private static void UpdateRange(Gravsphere gravsphere)
        {
            if (!gravsphere.GetComponent<EnhancedGravSphere>())
            {
                return;
            }

            if (gravsphere.gameObject.GetComponents<SphereCollider>()?.FirstOrDefault(s => s.radius > 10) is SphereCollider sphere)
            {
                sphere.radius = Main_Plugin.EnhancedRange.Value;
            }
        }

        [HarmonyPatch(nameof(Gravsphere.Start)), HarmonyPostfix]
        private static void Start_Patch(Gravsphere __instance)
        {
            UpdateRange(__instance);

            if (!bufferedAttractables.ContainsKey(__instance))
            {
                bufferedAttractables.Add(__instance, new List<Rigidbody>());
            }

            if (bufferedAttractables[__instance] is null)
            {
                bufferedAttractables[__instance] = new List<Rigidbody>();
            }

            ResetTriggers[__instance] = false;
        }

        [HarmonyPatch(nameof(Gravsphere.Update)), HarmonyPostfix]
        private static void Update_Patch(Gravsphere __instance)
        {
            EnhancedGravSphere enhancedSphere = __instance.GetComponent<EnhancedGravSphere>();
            if (!enhancedSphere)
            {
                return;
            }

            StorageContainer container = __instance.GetComponentInChildren<StorageContainer>();
            HandleStorageOpening(__instance, container);

            HandleStorageAdding(__instance, container);
            HandleBufferTransfer(__instance, container);
            HandleHeldEnable(__instance);
            HandleHandText(__instance, container);
        }

        private static void HandleStorageAdding(Gravsphere instance, StorageContainer container)
        {   
            for (int i = 0; i < instance.attractableList.Count; i++)
            {
                Rigidbody rb = instance.attractableList[i];
                if(!rb)
                {
                    continue;
                }

                if (Vector3.Distance(rb.position, instance.transform.position) > Main_Plugin.GravStoragePickupDistance.Value)
                {
                    continue;
                }

                if (rb.TryGetComponent(out BreakableResource resource))
                {
                    resource.BreakIntoResources();
                }

                if (rb.TryGetComponent(out Pickupable pickupable))
                {
                    if (!container.container.HasRoomFor(pickupable))
                    {
                        continue;
                    }

                    pickupable.Initialize();
                    container.container.AddItem(pickupable);

                    int lastIndex = bufferedAttractables[instance].Count - 1;
                    if (lastIndex <= 0)
                    {
                        return;
                    }

                    Rigidbody bufferedRb = bufferedAttractables[instance][lastIndex];
                    if (!bufferedRb.isKinematic)
                    {
                        instance.AddAttractable(bufferedRb);
                        bufferedAttractables[instance].Remove(bufferedRb);
                    }
                    UWE.Utils.SetIsKinematicAndUpdateInterpolation(bufferedRb, false, false);
                }
            }
        }
        private static void HandleBufferTransfer(Gravsphere instance, StorageContainer container)
        {
            for (int i = bufferedAttractables[instance].Count - 1; i >= 0; i--)
            {
                Rigidbody rb = bufferedAttractables[instance][i];
                if(!rb)
                {
                    bufferedAttractables[instance].RemoveAt(i);
                    continue;
                }
                Pickupable pickupable = rb.GetComponent<Pickupable>();
                if (!pickupable)
                {
                    continue;
                }

                if (container.container.HasRoomFor(pickupable))
                {
                    instance.AddAttractable(rb);
                    bufferedAttractables[instance].RemoveAt(i);
                }
            }
        }
        private static void HandleHeldEnable(Gravsphere instance)
        {
            if (!Player.main.IsInside())
            {
                if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                {
                    if (!instance.trigger.enabled && !ResetTriggers[instance])
                    {
                        instance.trigger.enabled = true;
                        instance.gameObject.GetComponent<Pickupable>().attached = false;
                    }
                    else
                    {
                        instance.DeactivatePads();
                        instance.trigger.enabled = false;
                        ResetTriggers[instance] = false;
                        instance.gameObject.GetComponent<Pickupable>().attached = true;
                    }
                }
            }
            else if (instance.trigger.enabled)
            {
                instance.DeactivatePads();
                instance.trigger.enabled = false;
                instance.gameObject.GetComponent<Pickupable>().attached = true;
            }
        }
        private static void HandleStorageOpening(Gravsphere instance, StorageContainer container)
        {
            //If deployed, in range and open key pressed, open storage
            bool inRange = Vector3.Distance(instance.transform.position, Player.main.transform.position) <= Main_Plugin.GravStorageOpenDistance.Value;
            bool trapInInventory = Inventory.main.Contains(instance.pickupable);
            if (inRange && GameInput.GetButtonDown(GameInput.Button.AltTool) && !trapInInventory)
            {
                if (!instance.GetComponent<Pickupable>()._attached)
                {
                    container.Open();
                }
            }

            //If not deployed & open key pressed, open storage
            if (!container.GetOpen() && !IngameMenu.main.selected && GameInput.GetButtonDown(GameInput.Button.AltTool) && trapInInventory)
            {
                container.Open(instance.transform);
            }
        }
        private static void HandleHandText(Gravsphere instance, StorageContainer container)
        {
            if (Inventory.main.quickSlots.heldItem == null) return;

            if(!Inventory.main.quickSlots.heldItem.Equals(instance.pickupable.inventoryItem))
            {
                return;
            }

            string gravtrapactivate =
                    container.container.IsFull() ? "Cannot Activate, Storage is Full" :
                    !instance.trigger.enabled && !ResetTriggers[instance] ? "Activate Gravtrap" :
                    "Deactivate Gravtrap";

            string primaryString =
                $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Deploy Gravtrap", uGUI.FormatButton(GameInput.Button.RightHand))}";

            string secondaryString = 
                $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", gravtrapactivate, uGUI.FormatButton(GameInput.Button.LeftHand))}\n"+
                $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Open Storage", uGUI.FormatButton(GameInput.Button.AltTool))}";

            if(!container.GetOpen() && !IngameMenu.main.selected)
            {
                HandReticle.main.textUse = primaryString;
                HandReticle.main.textUseSubscript = secondaryString;
            }
        }

        #region ---Transpilers ---
        [HarmonyPatch(nameof(Gravsphere.OnTriggerEnter)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnTriggerEnter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S);

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, match)
                .Advance(-3)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop);

            return newInstructions.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(Gravsphere.ApplyGravitation)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ApplyGravitation_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch listMatch = new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "attractableList");
            CodeMatch callvirtMatch = new CodeMatch(i => i.opcode == OpCodes.Callvirt);
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 15f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, match)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<Gravsphere, float>>(GetMaxForce))
                .MatchForward(false, match)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<Gravsphere, float>>(GetMaxMassStable))
                .MatchForward(false, listMatch, callvirtMatch)
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .SetInstruction(Transpilers.EmitDelegate<Func<List<Rigidbody> ,Gravsphere, int>>(GetAllowedIterations));   

            return newInstructions.InstructionEnumeration();
        }

        public static int GetAllowedIterations(List<Rigidbody> rbs, Gravsphere gravsphere)
        {
            int maxAmount = 12;
            if(gravsphere.GetComponent<EnhancedGravSphere>())
            {
                maxAmount = Main_Plugin.EnhancedMaxObjects.Value;
            }

            return Mathf.Min(rbs.Count, maxAmount);
        }

        private static float GetMaxForce(Gravsphere sphere)
        {
            if (sphere.TryGetComponent<EnhancedGravSphere>(out EnhancedGravSphere component))
            {
                return Main_Plugin.EnhancedMaxForce.Value;
            }
            else
            {
                return 15f;
            }
        }

        private static float GetMaxMassStable(Gravsphere sphere)
        {
            if (sphere.TryGetComponent<EnhancedGravSphere>(out EnhancedGravSphere component))
            {
                return Main_Plugin.EnhancedMaxMassStable.Value;
            }
            else
            {
                return 15f;
            }
        }

        private static int GetMaxObjects(Gravsphere sphere)
        {
            if (sphere.TryGetComponent<EnhancedGravSphere>(out EnhancedGravSphere component))
            {
                return Main_Plugin.EnhancedMaxObjects.Value;
            }
            else
            {
                return 12;
            }
        }
        #endregion

        //Use animation from vanilla gravtrap
        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.SetAnimationState))]
        [HarmonyPrefix]
        private static bool Animation_Patch(QuickSlots __instance, string toolName)
        {
            if (toolName != ImprovedTrap_Craftable.techType.ToString().ToLower())
                return true;

            __instance.SetAnimationState("gravsphere");
            return false;
        }

        [HarmonyPatch(nameof(Gravsphere.OnTriggerEnter)), HarmonyPostfix]
        private static void TriggerEnterPostfix(Gravsphere __instance, Collider collider)
        {
            //Buffer stuff (Fixes dropping of quartz)
            if(!__instance.TryGetComponent<EnhancedGravSphere>(out _))
            {
                return;
            }

            Rigidbody rb = UWE.Utils.GetComponentInHierarchy<Rigidbody>(collider.gameObject);
            if (!rb)
            {
                return;
            }

            GravTrapObjectsType gravTrapObjectsType = __instance.GetComponent<GravTrapObjectsType>();
            if (!gravTrapObjectsType.IsValidTarget(collider.gameObject))
            {
                return;
            }

            if (!rb.isKinematic)
            {
                bufferedAttractables[__instance].Add(rb);
            }
        }
    }
}
