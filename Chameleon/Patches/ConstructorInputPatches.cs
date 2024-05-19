using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(ConstructorInput))]
    internal class ConstructorInputPatches
    {
        [HarmonyPatch(nameof(ConstructorInput.OnCraftingBeginAsync)), HarmonyPatch(MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnCraftingBeginAsync_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "Get");

            var matcher = new CodeMatcher(instructions)
                .MatchForward(false, match)
                .Advance(1)
                .Insert(Transpilers.EmitDelegate(EnsurePrefabEnabled));

            return matcher.InstructionEnumeration();
        }

        public static GameObject EnsurePrefabEnabled(GameObject prefab)
        {
            prefab.SetActive(true);
            return prefab;
        }
    }
}
