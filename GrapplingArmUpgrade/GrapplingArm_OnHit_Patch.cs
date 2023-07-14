using HarmonyLib;
using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    internal static class GrapplingArm_OnHit_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(ExosuitGrapplingArm.OnHit))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch hookSpeedMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 25f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, hookSpeedMatch)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<ExosuitGrapplingArm, float>>(GetHookSpeed));

            return newInstructions.InstructionEnumeration();
        }

        public static float GetHookSpeed(ExosuitGrapplingArm arm)
        {
            float defaultHookSpeed = 25f;
            float enhancedHookSpeed = Main_Plugin.HookSpeed.Value;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) > 0)
            {
                return enhancedHookSpeed;
            }
            else
            {
                return defaultHookSpeed;
            }
        }
    }
}
