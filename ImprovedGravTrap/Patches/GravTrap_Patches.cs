using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(Gravsphere))]
    internal static class GravTrap_Patches
    {
        private static readonly Dictionary<Gravsphere, List<Rigidbody>> bufferedAttractables = new Dictionary<Gravsphere, List<Rigidbody>>();

        private static void UpdateRange(Gravsphere gravsphere)
        {
            if(!gravsphere.GetComponent<EnhancedGravSphere>())
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
        }

        [HarmonyPatch(nameof(Gravsphere.Update)), HarmonyPostfix]
        private static void Update_Patch(Gravsphere __instance)
        {
            if(!__instance.GetComponent<EnhancedGravSphere>())
            {
                return;
            }

            bool inRange = Vector3.Distance(__instance.transform.position, Player.main.transform.position) <= Main_Plugin.GravStorageOpenDistance.Value;
            StorageContainer container = __instance.GetComponentInChildren<StorageContainer>();

            if (inRange && Input.GetKeyDown(Main_Plugin.OpenStorageKey.Value))
            {
                if(!__instance.GetComponent<Pickupable>()._attached)
                {
                    container.Open();
                }
            }

            for (int i = 0; i < __instance.attractableList.Count; i++)
            {
                Rigidbody rb = __instance.attractableList[i];

                if (Vector3.Distance(rb.position, __instance.transform.position) > Main_Plugin.GravStoragePickupDistance.Value)
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

                    int lastIndex = bufferedAttractables[__instance].Count - 1;
                    if(lastIndex <= 0)
                    {
                        return;
                    }

                    Rigidbody bufferedRb = bufferedAttractables[__instance][lastIndex];
                    if(!bufferedRb.isKinematic)
                    {
                        __instance.AddAttractable(bufferedRb);
                        bufferedAttractables[__instance].Remove(bufferedRb);
                    }
                    UWE.Utils.SetIsKinematicAndUpdateInterpolation(bufferedRb, false, false);
                }
            }

            for (int i = bufferedAttractables[__instance].Count - 1; i >= 0; i--)
            {
                Rigidbody rb = bufferedAttractables[__instance][i];
                Pickupable pickupable = rb.GetComponent<Pickupable>();
                if(!pickupable)
                {
                    continue;
                }

                if(container.container.HasRoomFor(pickupable))
                {
                    __instance.AddAttractable(rb);
                    bufferedAttractables[__instance].RemoveAt(i);
                }
            }
        }
        
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

            foreach (var inst in newInstructions.InstructionEnumeration())
            {
                Main_Plugin.logger.LogInfo(inst.ToString());
            }

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

        /*
        [HarmonyPatch(nameof(Gravsphere.ApplyGravitation)), HarmonyPrefix]
        private static bool ApplyGravitation_Prefix(Gravsphere __instance)
        {
            Pickupable pickupable = __instance.GetComponent<Pickupable>();

            return pickupable._attached ? false : true;
        }

        [HarmonyPatch(nameof(Gravsphere.OnTriggerEnter)), HarmonyPrefix]
        private static bool OnTriggerEnter_Prefix(Gravsphere __instance)
        {
            Pickupable pickupable = __instance.GetComponent<Pickupable>();

            return pickupable._attached ? false : true;
        }
        */

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

        //Use animation from vanilla gravtrap
        [HarmonyPrefix, HarmonyPatch(typeof(QuickSlots), "SetAnimationState")]
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
