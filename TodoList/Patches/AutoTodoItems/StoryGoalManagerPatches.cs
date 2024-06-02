using HarmonyLib;
using Story;
using System.Linq;
using TodoList.Monobehaviors;

namespace TodoList.Patches.AutoTodoItems
{
    [HarmonyPatch(typeof(StoryGoalManager))]
    internal class StoryGoalManagerPatches
    {
        [HarmonyPatch(nameof(StoryGoalManager.OnGoalComplete)), HarmonyPostfix]
        private static void OnGoalComplete_Postfix(StoryGoalManager __instance, bool __result, string key)
        {
            if (!__result) return;

            //Main_Plugin.logger.LogInfo($"OnGoalComplete for {key}");

            Main_Plugin.StoryGoalTodoEntry entry = Main_Plugin.StoryGoalTodoEntries.FirstOrDefault(i => i.key == key);

            if (Main_Plugin.StoryGoalTodoEntries.Any(i => i.key == key))
            {
                ErrorMessage.AddError($"<color=#FFFF00>{Language.main.Get("TODO_NewHintItems")}</color>");

                string[] entries = entry.entries;
                if (entry.localized)
                {
                    for (int i = 0; i < entries.Length; i++)
                    {
                        entries[i] = Language.main.Get(entries[i]);
                    }
                }

                uGUI_TodoTab.Instance.CreateNewItems(entries, true);
            }
        }
    }
}
