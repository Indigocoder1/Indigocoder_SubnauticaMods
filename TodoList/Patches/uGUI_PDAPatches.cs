using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TodoList.Monobehaviors;
using UnityEngine;

namespace TodoList.Patches
{
    [HarmonyPatch(typeof(uGUI_PDA))]
    internal class uGUI_PDAPatches
    {
        [HarmonyPatch(nameof(uGUI_PDA.Initialize)), HarmonyPrefix]
        private static void Initialize_Prefix(uGUI_PDA __instance)
        {
            if (uGUI_PDA.regularTabs.Contains(Main_Plugin.todoTab)) return;

            uGUI_PDA.regularTabs.Add(Main_Plugin.todoTab);
        }

        [HarmonyPatch(nameof(uGUI_PDA.Initialize)), HarmonyPostfix]
        private static void Initialize_Postfix(uGUI_PDA __instance)
        {
            GameObject logTab = __instance.tabLog.gameObject;
            GameObject todoTab = GameObject.Instantiate(logTab, __instance.transform.Find("Content"));
            todoTab.name = "TodoTab";
            GameObject.DestroyImmediate(todoTab.GetComponent<uGUI_LogTab>());
            todoTab.AddComponent<uGUI_TodoTab>();

            __instance.tabs.Add(Main_Plugin.todoTab, todoTab.GetComponent<uGUI_PDATab>());

            TodoItem.todoItems = new();
            todoTab.GetComponent<uGUI_TodoTab>().LoadSavedItems();
        }

        [HarmonyPatch(nameof(uGUI_PDA.SetTabs)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> SetTabs_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatch[] matches = new CodeMatch[]
            {
                new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "Get"),
                new(i=> i.opcode == OpCodes.Stelem_Ref)
            };

            var newInstructions = new CodeMatcher(instructions)
                .MatchForward(false, matches)
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
                .Insert(Transpilers.EmitDelegate(TryGetTodoListSprite));

            return newInstructions.InstructionEnumeration();
        }

        public static Atlas.Sprite TryGetTodoListSprite(Atlas.Sprite originalSprite, PDATab currentTab)
        {
            if (currentTab != Main_Plugin.todoTab) return originalSprite;

            return Main_Plugin.TodoListTabSprite;
        }
    }
}
