using Chameleon.Monobehaviors;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubrootPatches
    {
        [HarmonyPatch(nameof(SubRoot.OnProtoDeserialize)), HarmonyPrefix]
        private static bool Deserialize_Prefix(SubRoot __instance)
        {
            if (__instance is not ChameleonSubRoot chameleon) return true;

            chameleon.LoadSaveData(chameleon.GetComponent<PrefabIdentifier>().Id);

            return false; //Temp fix until SubNames are properly implemented (idek if this will get done, just copying the Seal's comments)
        }

        [HarmonyPatch(nameof(SubRoot.OnTakeDamage)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnTakeDamage_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //This is needed so that the destruction event isn't called twice (Once in the SubRoot and once in the ChameleonSubRoot)
            //I could just rename the methods and hide the inherited old one but I'd rather do this
            //Also, Idk why the "i.operand is ___" is needed, but harmony throws invalid cast errors without them
            CodeMatch[] matches = new CodeMatch[]
            {
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldstr && (i.operand is string) && ((string)i.operand == "DestroyCyclopsSubRoot")),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && (i.operand is float) && ((float)i.operand == 18f))
            };

            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchForward(false, matches)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop));

            return matcher.InstructionEnumeration();
        }
    }
}
