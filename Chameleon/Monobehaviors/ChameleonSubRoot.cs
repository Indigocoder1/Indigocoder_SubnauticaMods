using Chameleon.Interfaces;
using Nautilus.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    public class ChameleonSubRoot : SubRoot, IProtoEventListener
    {
        private SaveData _saveData;
        public SaveData SaveData { get => _saveData; }

        [Header("Chameleon Info")]
        public ChameleonUpgradeConsole chameleonUpgradeConsole;
        public GameObject moduleFunctionsRoot;

        private ToggleLights toggleLights;

        internal static Dictionary<TechType, Type> moduleFunctions = new();

        public void AddIsAllowedToAddListener(IsAllowedToAdd @delegate)
        {
            chameleonUpgradeConsole.modules.isAllowedToAdd += @delegate;
        }

        public void RemoveIsAllowedToAddListener(IsAllowedToAdd @delegate)
        {
            chameleonUpgradeConsole.modules.isAllowedToAdd -= @delegate;
        }

        public override void Awake()
        {
            base.Awake();
            _saveData = new SaveData(); //New entry for the sub
        }

        public override void Start()
        {
            chameleonUpgradeConsole.modules.onEquip += OnEquip;
            chameleonUpgradeConsole.modules.onUnequip += OnUnequip;
            toggleLights = GetComponent<ToggleLights>();

            base.Start();
        }

        private void Update()
        {
            base.Update();

            if (Player.main.currentSub != this) return;

            toggleLights.CheckLightToggle();
        }

        private void OnEnable() => Main_Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
        private void OnDisable() => Main_Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;

        private void OnBeforeSave(object _, JsonFileEventArgs __)
        {
            Main_Plugin.SaveCache.saves[GetComponent<PrefabIdentifier>().Id] = SaveData;
        }

        internal void LoadSaveData(string id)
        {
            if(!Main_Plugin.SaveCache.saves.TryGetValue(id, out var data))
            {
                Main_Plugin.logger.LogError($"Chameleon {id} tried to load save data but none was found! This means there's a mismatch in the Ids, and possibly a race condition");
                return;
            }

            _saveData = data;

            foreach (var saveListener in GetComponentsInChildren<IOnSaveDataLoaded>(true))
            {
                saveListener.OnSaveDataLoaded(SaveData);
            }
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            Type type = moduleFunctions[item.item.GetTechType()];
            Component component = moduleFunctionsRoot.AddComponent(type);
            NotifyOnChange(item.item.GetTechType(), true);
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            Type type = moduleFunctions[item.item.GetTechType()];
            Component component = moduleFunctionsRoot.GetComponent(type);
            (component as MonoBehaviour).enabled = false;
            Destroy(component);
            NotifyOnChange(item.item.GetTechType(), false);
        }

        private void NotifyOnChange(TechType type, bool added)
        {
            foreach (var onModuleChange in moduleFunctionsRoot.GetComponents<IOnModuleChange>())
            {
                if((onModuleChange as MonoBehaviour).enabled)
                {
                    onModuleChange.OnChange(type, added);
                }
            }
        }
    }
}
