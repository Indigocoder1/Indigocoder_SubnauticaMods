using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace UpgradedJumpJetModule
{
    [HarmonyPatch(typeof(Exosuit))]
    internal static class Exosuit_FixedUpdate_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Exosuit.FixedUpdate))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch codeMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 8.5f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, codeMatch)
                .SetOpcodeAndAdvance(OpCodes.Ldarg_0)
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exosuit_FixedUpdate_Patch), nameof(GetJetForce), parameters: new Type[] { typeof(Exosuit) })));

            return newInstructions.InstructionEnumeration();
        }
        
        public static float GetJetForce(Exosuit exosuit)
        {
            float noJetsForce = 6.5f;
            float normalJetsForce = 8.5f;
            float upgradedForce = Main_Plugin.UpgradedJetForce.Value;

            if (exosuit.modules.GetCount(UpgradedJetsModule.techType) > 0)
            {
                return upgradedForce;
            }
            else if(exosuit.modules.GetCount(TechType.ExosuitJetUpgradeModule) > 0 && exosuit.modules.GetCount(UpgradedJetsModule.techType) == 0)
            {
                return normalJetsForce;
            }

            return noJetsForce;
        }
    }
}
