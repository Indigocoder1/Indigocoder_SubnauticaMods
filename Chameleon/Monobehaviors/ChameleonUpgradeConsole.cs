using Chameleon.Interfaces;
using Nautilus.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace Chameleon.Monobehaviors
{
    public class ChameleonUpgradeConsole : HandTarget, IHandTarget, IOnSaveDataLoaded
    {
        public Equipment modules { get; private set; }

        public GameObject modulesRoot;
        public GameObject[] moduleModels;
        public string[] slots;
        
        public override void Awake()
        {
            base.Awake();
            if(modules == null)
            {
                InitializeModules();
            }
        }

        private void InitializeModules()
        {
            modules = new Equipment(gameObject, modulesRoot.transform);
            modules.SetLabel("CyclopsUpgradesStorageLabel");
            UpdateVisuals();
            modules.onEquip += OnEquip;
            modules.onUnequip += OnUnequip;
            modules.AddSlots(slots);
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            UpdateVisuals();
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            for (int i = 0; i < moduleModels.Length; i++)
            {
                SetModuleVisibility(slots[i], moduleModels[i]);
            }
        }

        private void SetModuleVisibility(string slot, GameObject module)
        {
            if(module)
            {
                module.SetActive(modules.GetTechTypeInSlot(slot) != TechType.None);
            }
        }

        public void OnHandClick(GUIHand hand)
        {
            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(modules);
            pda.Open(PDATab.Inventory);
        }

        public void OnHandHover(GUIHand hand)
        {
            HandReticle main = HandReticle.main;
            main.SetText(HandReticle.TextType.Hand, "UpgradeConsole", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnSaveDataLoaded(SaveData saveData)
        {
            if (!saveData.modules.TryGetValue(gameObject.name, out var value)) return;

            CoroutineHost.StartCoroutine(SpawnSavedModules(value));
        }

        private IEnumerator SpawnSavedModules(Dictionary<string, TechType> cachedModules)
        {
            if (modules == null)
            {
                InitializeModules();
            }

            foreach (var module in cachedModules)
            {
                if (module.Value == TechType.None) continue;

                var task = CraftData.GetPrefabForTechTypeAsync(module.Value);
                yield return task;

                GameObject newModule = GameObject.Instantiate(task.GetResult(), modulesRoot.transform);
                newModule.SetActive(false);
                modules.AddItem(module.Key, new InventoryItem(newModule.GetComponent<Pickupable>()), true);
            }
        }

        private void OnEnable()
        {
            Main_Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
        }

        private void OnDisable()
        {
            Main_Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;
        }

        public void OnBeforeSave(object sender, JsonFileEventArgs args)
        {
            var saveData = GetComponentInParent<ChameleonSubRoot>().SaveData;
            var newModules = new Dictionary<string, TechType>();

            foreach (var item in modules.equipment)
            {
                newModules.Add(item.Key, item.Value != null ? item.Value.item.GetTechType() : TechType.None);
            }

            saveData.modules[gameObject.name] = newModules;
        }
    }
}
