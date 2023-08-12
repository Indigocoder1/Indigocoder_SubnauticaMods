using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ImprovedGravTrap;
using UnityEngine;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(Gravsphere))]
    internal static class GravTrap_Patches
    {
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
        private static void Start(Gravsphere __instance)
        {
            UpdateRange(__instance);
        }

        [HarmonyPatch(nameof(Gravsphere.OnTriggerEnter)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnTriggerEnter(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S);

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, match)
                .SetOpcodeAndAdvance(OpCodes.Ldarg_0)
                .Insert(Transpilers.EmitDelegate<Func<Gravsphere, int>>(GetMaxObjects));

            return newInstructions.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(Gravsphere.ApplyGravitation)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ApplyGravitation(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 15f));

            var newInstructions = new CodeMatcher(instructions)
                            .MatchForward(false, match)
                            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                            .Insert(Transpilers.EmitDelegate<Func<Gravsphere, float>>(GetMaxForce))
                            .MatchForward(false, match)
                            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                            .Insert(Transpilers.EmitDelegate<Func<Gravsphere, float>>(GetMaxMassStable));

            return newInstructions.InstructionEnumeration();
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

        //Use animation from vanilla gravtrap
        [HarmonyPrefix, HarmonyPatch(typeof(QuickSlots), "SetAnimationState")]
        static bool patchAnimation(QuickSlots __instance, string toolName)
        {
            if (toolName != Trap_Craftable.techType.ToString().ToLower())
                return true;

            __instance.SetAnimationState("gravsphere");
            return false;
        }
    }
}
