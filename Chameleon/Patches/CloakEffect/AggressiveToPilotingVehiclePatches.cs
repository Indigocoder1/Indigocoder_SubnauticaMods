using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using Chameleon.Monobehaviors;

namespace Chameleon.Patches.CloakEffect
{
    [HarmonyPatch(typeof(AttackCyclops))]
    internal class AggressiveToPilotingVehiclePatches
    {
        private static CodeMatch[] Matches 
        {
            get
            {
                if(_matches != null)
                {
                    return _matches;
                }

                if (Main_Plugin.SealSubInstalled)
                {
                    _matches = new CodeMatch[]
                    {
                    new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "IsCyclopsBool")
                    };
                }
                else
                {
                    _matches = new CodeMatch[]
                    {
                    new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(Player), nameof(Player.main))),
                    new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Player), nameof(Player.currentSub))),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SubRoot), nameof(SubRoot.isCyclops)))
                    };
                }

                return _matches;
            }
        }

        private static CodeMatch[] _matches;

        public static bool IsCurrentSubCyclops(bool resultWasCyclops)
        {
            //Compatibility with SealSub and other mods that may change this
            SubRoot currentSub = Player.main.currentSub;
            if(currentSub is not ChameleonSubRoot)
            {
                return resultWasCyclops;
            }

            return !(currentSub as ChameleonSubRoot).camoButton.IsCamoActive();
        }

        [HarmonyPatch(nameof(AttackCyclops.UpdateAggression)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> UpdateAggression_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false, Matches)
                .Advance(1);

            if(!Main_Plugin.SealSubInstalled)
            {
                matcher.Advance(2);
            }

            matcher.Insert(Transpilers.EmitDelegate(IsCurrentSubCyclops));

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(AttackCyclops.OnCollisionEnter)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnCollisionEnter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false, Matches)
                .Advance(1);

            if (!Main_Plugin.SealSubInstalled)
            {
                matcher.Advance(2);
            }

            matcher.Insert(Transpilers.EmitDelegate(IsCurrentSubCyclops));

            return matcher.InstructionEnumeration();
        }
    }
}
