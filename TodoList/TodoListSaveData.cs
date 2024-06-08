using Nautilus.Json;
using Nautilus.Json.Attributes;
using System;
using System.Collections.Generic;
using TodoList.Monobehaviors;
using static TodoList.Main_Plugin;

namespace TodoList
{
    [Serializable]
    public struct ItemSaveData
    {
        public string itemBody;
        public bool isCompleted;
        public bool isHint;
        public EntryInfo entryInfo;

        public ItemSaveData(string itemBody, bool isCompleted, bool isHint, EntryInfo entryInfo)
        {
            this.itemBody = itemBody;
            this.isCompleted = isCompleted;
            this.isHint = isHint;
            this.entryInfo = entryInfo;
        }
    }

    [FileName("TodoListSaveData")]
    public class TodoListSaveData : SaveDataCache
    {
        public List<ItemSaveData> saveData = new();

        public TodoListSaveData()
        {
            OnStartedSaving += (_, __) =>
            {
                List<ItemSaveData> data = new();
                foreach (var item in TodoItem.todoItems)
                {
                    Main_Plugin.logger.LogInfo($"Saving data for {item}");
                    data.Add(item.saveData);
                }

                saveData = data;
            };
        }
    }
}
