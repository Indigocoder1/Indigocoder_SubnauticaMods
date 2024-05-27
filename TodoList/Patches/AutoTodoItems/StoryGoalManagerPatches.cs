using HarmonyLib;
using Story;
using TodoList.Monobehaviors;

namespace TodoList.Patches.AutoTodoItems
{
    [HarmonyPatch(typeof(StoryGoalManager))]
    internal class StoryGoalManagerPatches
    {
        [HarmonyPatch(nameof(StoryGoalManager.OnGoalComplete)), HarmonyPostfix]
        private static void OnGoalComplete_Postfix(StoryGoalManager __instance, bool __result, string key)
        {
            Main_Plugin.logger.LogInfo($"OnGoalComplete for {key}");

            if (Main_Plugin.StoryGoalTodoEntries.TryGetValue(key, out string[] entries))
            {
                ErrorMessage.AddError(Language.main.Get("TODO_NewHintItems"));
                uGUI_TodoTab.Instance.CreateNewItems(entries, true);
            }
        }
    }
}
