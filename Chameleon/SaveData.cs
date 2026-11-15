using Nautilus.Json;
using Nautilus.Json.Attributes;
using System;
using System.Collections.Generic;

namespace Chameleon
{
    [Serializable]
    public class SaveData
    {
        public Dictionary<string, Dictionary<string, TechType>> modules = new();
    }

    [FileName("ChameleonSaveData")]
    internal class ChameleonSaveCache : SaveDataCache
    {
        public Dictionary<string, SaveData> saves = new();

        public ChameleonSaveCache()
        {
            OnFinishedLoading += (object _, JsonFileEventArgs _) => saves.ForEach((entry) =>
            {
                Main_Plugin.logger.LogMessage($"Finished loading item. Key: {entry.Key}, value: {entry.Value}");  
            });
        }
    }
}
