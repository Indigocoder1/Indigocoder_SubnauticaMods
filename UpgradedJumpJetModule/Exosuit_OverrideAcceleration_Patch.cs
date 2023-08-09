using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace UpgradedJumpJetModule
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_OverrideAcceleration_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Exosuit.OverrideAcceleration))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch codeMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 0.3f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, codeMatch)
                .SetOpcodeAndAdvance(OpCodes.Ldarg_0)
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exosuit_OverrideAcceleration_Patch), nameof(GetAccelerationForce), parameters: new Type[] { typeof(Exosuit) })));

            return newInstructions.InstructionEnumeration();
        }

        public static float GetAccelerationForce(Exosuit exosuit)
        {
            float noJetsForce = 0.22f;
            float normalJetsForce = 0.3f;
            float upgradedForce = Main_Plugin.UpgradedJetAcceleration.Value;

            if (exosuit.modules.GetCount(UpgradedJetsModule.techType) > 0)
            {
                return upgradedForce;
            }
            else if (exosuit.modules.GetCount(TechType.ExosuitJetUpgradeModule) > 0 && exosuit.modules.GetCount(UpgradedJetsModule.techType) == 0)
            {
                return normalJetsForce;
            }

            return noJetsForce;
        }
    }
}
