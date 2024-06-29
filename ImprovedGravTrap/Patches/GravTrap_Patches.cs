using HarmonyLib;
using ImprovedGravTrap.Monobehaviours;
using Nautilus.Utility;
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
                .Insert(Transpilers.EmitDelegate(GetMaxForce))
                .MatchForward(false, match)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate(GetMaxMassStable))
                .MatchForward(false, listMatch, callvirtMatch)
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .SetInstruction(Transpilers.EmitDelegate(GetAllowedIterations));   

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
            if (sphere.TryGetComponent(out EnhancedGravSphere component))
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
            if (sphere.TryGetComponent(out EnhancedGravSphere component))
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
            if (sphere.TryGetComponent(out EnhancedGravSphere component))
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
            if(!__instance.TryGetComponent(out EnhancedGravSphere enhancedGravSphere))
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
                enhancedGravSphere.bufferedAttractables.Add(rb);
            }
        }
    }
}
