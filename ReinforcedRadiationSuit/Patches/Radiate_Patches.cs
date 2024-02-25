using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ReinforcedRadiationSuit.Items;
using UnityEngine;

namespace ReinforcedRadiationSuit.Patches
{
    [HarmonyPatch(typeof(RadiatePlayerInRange))]
    internal static class Radiate_Patches
    {
        [HarmonyPatch(nameof(RadiatePlayerInRange.Radiate)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Radiate_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch getEquipmentMatch = new CodeMatch(i => i.opcode == OpCodes.Callvirt && (((MethodInfo)i.operand).Name == "get_equipment"));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, getEquipmentMatch)
                .Advance(2)
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(GetMaxCountBetweenTechTypes))
                .MatchForward(false, getEquipmentMatch)
                .Advance(2)
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(GetMaxCountBetweenTechTypes))
                .MatchForward(false, getEquipmentMatch)
                .Advance(2)
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(GetMaxCountBetweenTechTypes));

            return newInstructions.InstructionEnumeration();
        }

        public static int GetMaxCountBetweenTechTypes(Equipment instance, int originalTechType)
        {
            TechType reinforcedTechType = (TechType)originalTechType switch
            {
                TechType.RadiationSuit => ReinforcedRadiationSuit_Craftable.techType,
                TechType.RadiationGloves => ReinforcedRadiationGloves_Craftable.techType,
                TechType.RadiationHelmet => RebreatherRadiationHelmet_Craftable.techType,
                _ => TechType.None
            };

            int originalCount = instance.GetCount((TechType)originalTechType);
            int reinforcedCount = instance.GetCount(reinforcedTechType);

            int maxBetweenBoth = Mathf.Max(originalCount, reinforcedCount);

            return maxBetweenBoth;
        }
    }
}
