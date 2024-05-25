using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace TodoList.Patches
{
    internal class MapModCompatibilityPatches
    {
        internal static IEnumerable<CodeInstruction> ControllerUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch[] matches = new CodeMatch[]
            {
                new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_Settings"),
                new(i => i.opcode == OpCodes.Ldfld),
                new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetKeyDown")
            };

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, matches)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4, 1));

            foreach (var instruction in newInstructions.InstructionEnumeration())
            {
                Main_Plugin.logger.LogInfo($"{instruction.opcode} {instruction.operand}");
            }

            return newInstructions.InstructionEnumeration();
        }
    }
}
