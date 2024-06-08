using HarmonyLib;
using Story;
using System.Diagnostics.Contracts;
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

            if (!Main_Plugin.CreateHintTodoItems.Value) return;

            Main_Plugin.logger.LogInfo($"On goal complete for {key}");

            if (!Main_Plugin.StoryGoalTodoEntries.Any(i => i.key == key)) return;

            Main_Plugin.StoryGoalTodoEntry entry = Main_Plugin.StoryGoalTodoEntries.FirstOrDefault(i => i.key == key);

            if(Main_Plugin.StoryGoalTodoEntries.Any(i => i.entryInfos.Any(i => i.completeKey == key)))
            {
                Main_Plugin.StoryGoalTodoEntry storyEntry = Main_Plugin.StoryGoalTodoEntries.First(i => i.entryInfos.Any(j => j.completeKey == key));
                Main_Plugin.EntryInfo entryInfo = storyEntry.entryInfos.FirstOrDefault(i => i.completeKey == key);
                uGUI_TodoTab.Instance.CompleteTodoItem(entryInfo);
            }

            Main_Plugin.EntryInfo[] entryInfos = entry.entryInfos;
            for (int i = 0; i < entryInfos.Length; i++)
            {
                Main_Plugin.logger.LogInfo($"Entry info complete key = {entryInfos[i].completeKey}");

                if (!entryInfos[i].localized) continue;

                entryInfos[i].entry = Language.main.Get(entryInfos[i].entry);
            }

            uGUI_TodoTab.Instance.CreateNewItems(entryInfos, true);

            ErrorMessage.AddError($"<color=#FFFF00>{Language.main.Get("TODO_NewHintItems")}</color>");
        }
    }
}
