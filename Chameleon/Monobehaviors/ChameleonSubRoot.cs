using Chameleon.Interfaces;
using Chameleon.Monobehaviors.UI;
using Nautilus.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    public class ChameleonSubRoot : SubRoot, IProtoEventListener
    {
        private const float SUB_EXPLOSION_DELAY = 13f;

        private SaveData _saveData;
        public SaveData SaveData { get => _saveData; }

        [Header("Chameleon Info")]
        public ChameleonCamoButton camoButton;
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

        new private void Awake()
        {
            _saveData = new SaveData(); //New entry for the sub
        }

        public override void Start()
        {
            chameleonUpgradeConsole.modules.onEquip += OnEquip;
            chameleonUpgradeConsole.modules.onUnequip += OnUnequip;
            toggleLights = GetComponent<ToggleLights>();

            GetComponentInChildren<SubFloodAlarm>().NewAlarmState();

            base.Start();
        }

        public void DestroyCyclopsSubRoot()
        {
            silentRunning = false;

            //Calls ChameleonDestructionEvent.DestroyChameleon
            SendMessage("DestroyChameleon");
        }

        new private void Update()
        {
            if (Player.main.currentSub != this) return;
            if (!Player.main.isPiloting) return;

            toggleLights.CheckLightToggle();
        }

        new public void OnKill()
        {
            voiceNotificationManager.ClearQueue();
            voiceNotificationManager.PlayVoiceNotification(abandonShipNotification);

            Destroy(GetComponent<RespawnPoint>());
            Invoke(nameof(DestroyCyclopsSubRoot), SUB_EXPLOSION_DELAY);
            MainCameraControl.main.ShakeCamera(1.5f, 20f);
            Player.main.TryEject();

            subWarning = true;
            BroadcastMessage("NewAlarmState", SendMessageOptions.DontRequireReceiver);

            //Update UI 1 extra time to fix stuff like the health not fully going down
            BroadcastMessage("OnChameleonDestroyed", SendMessageOptions.DontRequireReceiver);
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
            moduleFunctionsRoot.AddComponent(type);
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
