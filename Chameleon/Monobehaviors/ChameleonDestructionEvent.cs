﻿using Chameleon.Interfaces;
using System.Collections;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class ChameleonDestructionEvent : MonoBehaviour, ICyclopsReferencer
    {
        public SubRoot subRoot;
        public FMOD_CustomEmitter explodeSFX;
        public VFXController fxController;
        public PingInstance pingInstance;
        public Transform lootSpawnPoints;
        public GameObject[] intact;
        public GameObject[] destroyed;
        public BatterySource[] powerCells;
        public float swapModelsDelay;
        public float interiorFxDuration = 5f;
        public float interiorFxStartOffset = 10;
        public float interiorFxEndOffset = -5;
        public int numScrapMetalLoots;
        public int numComputerChipLoots;

        private GameObject interiorFxGO;
        [SerializeField] private GameObject interiorPlayerFx;
        private GameObject playerFxInstance;
        private GameObject beaconPrefab;
        private float animTime;

        private IEnumerator Start()
        {
            DevConsole.RegisterConsoleCommand(this, "destroychameleon");
            Player.main.playerRespawnEvent.AddHandler(gameObject, OnRespawn);

            if (subRoot.live.health <= 0f)
            {
                SwapToDamagedModels();
                ToggleSinking(true);
            }

            var request = CraftData.GetPrefabForTechTypeAsync(TechType.Beacon);
            yield return request;
            beaconPrefab = request.GetResult();
        }

        public void DestroyChameleon()
        {
            explodeSFX.Play();
            animTime = 0;

            GetComponentInChildren<PilotingChair>().enabled = false;
            GetComponentInChildren<Fabricator>().enabled = false;
            GetComponentsInChildren<Light>().ForEach(light => light.enabled = false);

            var identifier = subRoot.GetComponent<PrefabIdentifier>();
            Main_Plugin.SaveCache.saves.Remove(identifier.Id);
            Destroy(identifier);

            if (Player.main.currentSub == subRoot)
            {
                fxController.Play(0);
            }
            else
            {
                fxController.Play(1);
            }

            Player.main.TryEject();
            interiorFxGO = fxController.emitters[0].instanceGO;
            Invoke(nameof(SwapToDamagedModels), swapModelsDelay);
            StartCoroutine(SpawnLootAsync());
            powerCells.ForEach(i => i.ConsumeEnergy(99999f, out _));
            if(beaconPrefab != null)
            {
                Vector3 beaconPos = subRoot.transform.position + new Vector3(0, 15f, 0);
                Beacon beacon = Instantiate(beaconPrefab, beaconPos, Quaternion.identity).GetComponent<Beacon>();
                if(beacon != null)
                {
                    string beaconName = Language.main.Get("ChameleonDamageLabel");
                    beacon.label = beaconName;
                }
            }
        }

        private void Update()
        {
            if(Player.main.currentSub != subRoot)
            {
                return;
            }

            if(interiorFxGO != null)
            {
                animTime += Time.deltaTime;
                Vector3 localPos = interiorFxGO.transform.localPosition;
                localPos.z = Mathf.Lerp(interiorFxStartOffset, interiorFxEndOffset, Mathf.Clamp01(animTime / interiorFxDuration));
                interiorFxGO.transform.localPosition = localPos;
                if(animTime > interiorFxDuration)
                {
                    fxController.Stop(0);
                }

                Vector3 localPlayerPos = Utils.GetLocalPlayerPos();
                if(interiorFxGO.transform.InverseTransformPoint(localPlayerPos).z < 23f)
                {
                    playerFxInstance = Utils.SpawnPrefabAt(interiorPlayerFx, SNCameraRoot.main.transform, SNCameraRoot.main.transform.position);
                    playerFxInstance.transform.localPosition = new Vector3(0f, 0f, 4f);
                    fxController.Stop(0);
                    interiorFxGO = null;
                    Player.main.liveMixin.Kill(DamageType.Normal);
                }
            }

            if (fxController.emitters[0].fxPS != null && fxController.emitters[0].fxPS.isPlaying)
            {
                fxController.Stop(0);
            }
        }

        public void OnRespawn(Player p)
        {
            fxController.StopAndDestroy(0, 0f);
            Destroy(playerFxInstance);
        }

        private void SwapToDamagedModels()
        {
            for (int i = 0; i < intact.Length; i++)
            {
                intact[i].SetActive(false);
            }

            for (int i = 0; i < destroyed.Length; i++)
            {
                destroyed[i].SetActive(true);
            }

            ToggleSinking(true);
            subRoot.subWarning = false;
            subRoot.BroadcastMessage("NewAlarmState", null, SendMessageOptions.DontRequireReceiver);
            subRoot.subDestroyed = true;
            pingInstance.enabled = false;
        }

        private void ToggleSinking(bool isSinking)
        {
            subRoot.worldForces.underwaterGravity = isSinking ? 3f : 0f;
        }

        private IEnumerator SpawnLootAsync()
        {
            yield return new WaitForSeconds(swapModelsDelay + 0.5f);
            var request = CraftData.GetPrefabForTechTypeAsync(TechType.ScrapMetal);
            yield return request;

            GameObject scrapMetal = request.GetResult();
            for (int i = 0; i < numScrapMetalLoots; i++)
            {
                Vector3 lootSpawnPoint = GetLootSpawnPoint();
                Vector3 offset = ((UnityEngine.Random.insideUnitSphere * 2f) - (Vector3.one * 1f)) * 15f;
                UWE.Utils.SetIsKinematic(Instantiate(scrapMetal, lootSpawnPoint + offset, UnityEngine.Random.rotation).GetComponent<Rigidbody>(), false);
            }

            request = CraftData.GetPrefabForTechTypeAsync(TechType.ComputerChip);
            yield return request;
            GameObject computerChip = request.GetResult();
            for (int i = 0; i < numComputerChipLoots; i++)
            {
                Vector3 lootSpawnPoint = GetLootSpawnPoint();
                Vector3 offset = ((UnityEngine.Random.insideUnitSphere * 2f) - (Vector3.one * 1f)) * 15f;
                UWE.Utils.SetIsKinematic(Instantiate(computerChip, lootSpawnPoint + offset, UnityEngine.Random.rotation).GetComponent<Rigidbody>(), false);
            }
        }

        private Vector3 GetLootSpawnPoint()
        {
            int childCount = lootSpawnPoints.childCount;
            int num = UnityEngine.Random.Range(0, childCount);
            return lootSpawnPoints.GetChild(num).position;
        }

        private void OnConsoleCommand_destroychameleon(NotificationCenter.Notification n)
        {
            DestroyChameleon();
        }

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            interiorPlayerFx = Instantiate(cyclops.GetComponent<CyclopsDestructionEvent>().interiorPlayerFx);
            interiorPlayerFx.SetActive(false);
        }
    }
}
