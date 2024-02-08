using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using VariableGravityCannon.Monos;

namespace VariableGravityCannon.Patches
{
    [HarmonyPatch(typeof(RepulsionCannon))]
    internal static class RepulsionCannonPatch
    {
        [HarmonyPatch(nameof(RepulsionCannon.OnToolUseAnim)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnUse_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch codeMatch = new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "ConsumeEnergy");

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, codeMatch)
                .Advance(-1)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate(GetFireEnergyConsumption));

            return newInstructions.InstructionEnumeration();
        }

        public static float GetFireEnergyConsumption(RepulsionCannon instance)
        {
            if (instance.GetComponent<VariableGravityCannon_Mono>() == null) return 4f;

            return Main_Plugin.RepulsionModeFireEnergy.Value;
        }
    }
}
