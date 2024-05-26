using Nautilus.Json;
using Nautilus.Json.Attributes;
using System;
using System.Collections.Generic;
using TodoList.Monobehaviors;

namespace TodoList
{
    [Serializable]
    public struct SaveData
    {
        public string itemBody;
        public bool isCompleted;

        public SaveData(string itemBody, bool isCompleted)
        {
            this.itemBody = itemBody;
            this.isCompleted = isCompleted;
        }
    }

    [FileName("TodoListSaveData")]
    public class TodoListSaveData : SaveDataCache
    {
        public List<SaveData> saveData = new();

        public TodoListSaveData()
        {
            OnStartedSaving += (_, __) =>
            {
                List<SaveData> data = new();
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
