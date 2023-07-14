using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;

namespace GrapplingArmUpgrade_BepInEx
{
    [HarmonyPatch(typeof(ExosuitGrapplingArm))]
    internal static class GrapplingArm_FixedUpdate_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(ExosuitGrapplingArm.FixedUpdate))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch accelerationMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 15f));
            CodeMatch forceMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 400f));
            CodeMatch maxDistanceMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 35f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, accelerationMatch)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<ExosuitGrapplingArm, float>>(GetAcceleration))
                .MatchForward(false, forceMatch)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<ExosuitGrapplingArm, float>>(GetForce))
                .MatchForward(false, maxDistanceMatch)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<ExosuitGrapplingArm, float>>(GetMaxDistance));

            return newInstructions.InstructionEnumeration();
        }

        public static float GetAcceleration(ExosuitGrapplingArm arm)
        {
            float defaultAcceleration = 15f;
            float enhancedAcceleration = Main_Plugin.ExosuitAcceleration.Value;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) > 0)
            {
                return enhancedAcceleration;
            }
            else
            {
                return defaultAcceleration;
            }
        }
        public static float GetForce(ExosuitGrapplingArm arm)
        {
            float defaultForce = 400f;
            float enhancedForce = Main_Plugin.HookShootForce.Value;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) > 0)
            {
                return enhancedForce;
            }
            else
            {
                return defaultForce;
            }
        }
        public static float GetMaxDistance(ExosuitGrapplingArm arm)
        {
            float defaultMaxDistance = 35f;
            float enhancedMaxDistance = Main_Plugin.HookMaxDistance.Value;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) > 0)
            {
                return enhancedMaxDistance;
            }
            else
            {
                return defaultMaxDistance;
            }
        }
    }
}
