using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using VariableGravityCannon.Monos;

namespace VariableGravityCannon.Patches
{
    [HarmonyPatch(typeof(PropulsionCannon))]
    internal static class PropulsionCannonPatch
    {
        [HarmonyPatch(nameof(PropulsionCannon.OnShoot)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnShoot_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch codeMatch = new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "ConsumeEnergy");

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, codeMatch)
                .Advance(-1)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate(GetFireEnergyConsumption));

            return newInstructions.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(PropulsionCannon.Update)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch codeMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 0.7f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, codeMatch)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate(GetConstEnergyConsumption));

            return newInstructions.InstructionEnumeration();
        }

        public static float GetFireEnergyConsumption(RepulsionCannon instance)
        {
            if (instance.GetComponent<VariableGravityCannon_Mono>() == null) return 4f;

            return Main_Plugin.propulsionModeFireEnergy.Value;
        }

        public static float GetConstEnergyConsumption(RepulsionCannon instance)
        {
            if (instance.GetComponent<VariableGravityCannon_Mono>() == null) return 0.7f;

            return Main_Plugin.propulsionModePerSecondEnergy.Value;
        }
    }
}
