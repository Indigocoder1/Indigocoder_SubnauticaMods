using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Logging;
using System.Linq;
using static VFXParticlesPool;
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
            float enhancedAcceleration = 20f;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) < 1)
            {
                return defaultAcceleration;
            }
            else
            {
                return enhancedAcceleration;
            }
        }
        public static float GetForce(ExosuitGrapplingArm arm)
        {
            float defaultForce = 400f;
            float enhancedForce = 600f;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) < 1)
            {
                return defaultForce;
            }
            else
            {
                return enhancedForce;
            }
        }

        public static float GetMaxDistance(ExosuitGrapplingArm arm)
        {
            float defaultMaxDistance = 35f;
            float enhancedMaxDistance = 50f;

            if (arm.exosuit.modules.GetCount(GrapplingArmUpgradeModule.TechType) < 1)
            {
                return defaultMaxDistance;
            }
            else
            {
                return enhancedMaxDistance;
            }
        }
    }
}
