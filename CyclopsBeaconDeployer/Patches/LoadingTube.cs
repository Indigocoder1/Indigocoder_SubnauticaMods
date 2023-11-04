using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyLoadingTube))]
    internal static class LoadingTube
    {
        [HarmonyPatch(nameof(CyclopsDecoyLoadingTube.TryRemoveDecoyFromTube)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RemoveFromTube_Postfix(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch codeMatch = new CodeMatch(i => i.opcode == OpCodes.Ldloc_1);

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, codeMatch)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<CyclopsDecoyLoadingTube, string>>(GetSlot));

            return newInstructions.InstructionEnumeration();
        }

        public static string GetSlot(CyclopsDecoyLoadingTube instance)
        {
            DecoyManager_Patches.DecoyInfo decoyInfo = DecoyManager_Patches.decoyInfos[instance.subRoot];
            if (!decoyInfo.launchedBeacon)
            {
                Main_Plugin.logger.LogInfo($"Slot w/ decoy = {decoyInfo.slotWithDecoy}");
                return decoyInfo.slotWithDecoy;
            }

            return decoyInfo.slotWithBeacon;
        }
    }
}
