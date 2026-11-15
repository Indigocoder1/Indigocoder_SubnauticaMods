using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine.Rendering;

namespace BrineUIFix.Patches;

[HarmonyPatch(typeof(WBOIT))]
internal class WBOIT_Patches
{
    [HarmonyPatch(nameof(WBOIT.RebuildCommandBuffer)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RebuildCommandBuffer_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new[]
        {
            new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S),
            new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
            new CodeMatch(i => i.opcode == OpCodes.Ldfld)
        };

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(GetPreAlphaEvent))
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(GetPreAlphaEvent));

        return matcher.InstructionEnumeration();
    }

    public static int GetPreAlphaEvent(int oldEvent)
    {         
        var pass = CameraEvent.BeforeDepthTexture;
        Plugin.Logger.LogInfo($"Rebuilding command buffer. Overwriting pass to be {pass}");
        return (int)pass;
    }
}
