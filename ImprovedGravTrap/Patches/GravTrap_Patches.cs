using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace ImprovedGravTrap.Patches
{
    [HarmonyPatch(typeof(Gravsphere))]
    internal static class GravTrap_Patches
    {
        private static void UpdateRange(Gravsphere gravsphere)
        {
            if(!gravsphere.GetComponent<EnhancedGravSphere>())
            {
                return;
            }

            if (gravsphere.gameObject.GetComponents<SphereCollider>()?.FirstOrDefault(s => s.radius > 10) is SphereCollider sphere)
            {
                sphere.radius = Main_Plugin.EnhancedRange.Value;
            }
        }

        [HarmonyPatch(nameof(Gravsphere.Start)), HarmonyPostfix]
        private static void Start_Patch(Gravsphere __instance)
        {
            UpdateRange(__instance);
        }

        [HarmonyPatch(nameof(Gravsphere.Update)), HarmonyPostfix]
        private static void Update_Patch(Gravsphere __instance)
        {
            if(__instance.GetComponent<EnhancedGravSphere>() == null)
            {
                return;
            }

            bool inRange = Vector3.Distance(__instance.transform.position, Player.main.transform.position) <= Main_Plugin.GravStorageOpenDistance.Value;
            StorageContainer container = __instance.GetComponentInChildren<StorageContainer>();
            if (inRange && Input.GetKeyDown(Main_Plugin.OpenStorageKey.Value))
            {
                if(!__instance.GetComponent<Pickupable>()._attached)
                {
                    container.Open();
                }
            }

            for (int i = 0; i < __instance.attractableList.Count; i++)
            {
                Rigidbody rb = __instance.attractableList[i];

                if (Vector3.Distance(rb.position, __instance.transform.position) > Main_Plugin.GravStoragePickupDistance.Value)
                {
                    continue;
                }

                if (rb.TryGetComponent<Pickupable>(out Pickupable pickupable))
                {
                    pickupable.Initialize();
                    container.container.AddItem(pickupable);
                }

                if (rb.TryGetComponent<BreakableResource>(out BreakableResource resource))
                {
                    resource.BreakIntoResources();
                }
            }
        }

        [HarmonyPatch(nameof(Gravsphere.OnTriggerEnter)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnTriggerEnter_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S);

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, match)
                .SetOpcodeAndAdvance(OpCodes.Ldarg_0)
                .Insert(Transpilers.EmitDelegate<Func<Gravsphere, int>>(GetMaxObjects));

            return newInstructions.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(Gravsphere.ApplyGravitation)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ApplyGravitation_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 15f));

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, match)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<Gravsphere, float>>(GetMaxForce))
                .MatchForward(false, match)
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Transpilers.EmitDelegate<Func<Gravsphere, float>>(GetMaxMassStable));

            return newInstructions.InstructionEnumeration();
        }

        [HarmonyPatch(nameof(Gravsphere.ApplyGravitation)), HarmonyPrefix]
        private static bool ApplyGravitation_Prefix(Gravsphere __instance)
        {
            Pickupable pickupable = __instance.GetComponent<Pickupable>();

            return pickupable._attached ? false : true;
        }

        [HarmonyPatch(nameof(Gravsphere.OnTriggerEnter)), HarmonyPrefix]
        private static bool OnTriggerEnter_Prefix(Gravsphere __instance)
        {
            Pickupable pickupable = __instance.GetComponent<Pickupable>();

            return pickupable._attached ? false : true;
        }

        private static float GetMaxForce(Gravsphere sphere)
        {
            if (sphere.TryGetComponent<EnhancedGravSphere>(out EnhancedGravSphere component))
            {
                return Main_Plugin.EnhancedMaxForce.Value;
            }
            else
            {
                return 15f;
            }
        }
        private static float GetMaxMassStable(Gravsphere sphere)
        {
            if (sphere.TryGetComponent<EnhancedGravSphere>(out EnhancedGravSphere component))
            {
                return Main_Plugin.EnhancedMaxMassStable.Value;
            }
            else
            {
                return 15f;
            }
        }
        private static int GetMaxObjects(Gravsphere sphere)
        {
            if (sphere.TryGetComponent<EnhancedGravSphere>(out EnhancedGravSphere component))
            {
                return Main_Plugin.EnhancedMaxObjects.Value;
            }
            else
            {
                return 12;
            }
        }

        //Use animation from vanilla gravtrap
        [HarmonyPrefix, HarmonyPatch(typeof(QuickSlots), "SetAnimationState")]
        static bool Animation_Patch(QuickSlots __instance, string toolName)
        {
            if (toolName != ImprovedTrap_Craftable.techType.ToString().ToLower())
                return true;

            __instance.SetAnimationState("gravsphere");
            return false;
        }
    }
}
