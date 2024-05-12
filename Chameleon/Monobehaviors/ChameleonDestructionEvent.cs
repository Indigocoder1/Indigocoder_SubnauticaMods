using Chameleon.ScriptableObjects;
using System;
using System.Collections;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class ChameleonDestructionEvent : MonoBehaviour
    {
        public SubRoot subRoot;
        public ChameleonFMODAsset explodeSFX;
        public VFXController fxController;
        public Transform lootSpawnPoints;
        public BatterySource[] powerCells;
        public float swapModelsDelay;
        public float interiorFxDuration = 5f;
        public float interiorFxStartOffset = 10;
        public float interiorFxEndOffset = -5;
        public int numScrapMetalLoots;
        public int numComputerChipLoots;

        private GameObject interiorFxGO;
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

        public void OnKill()
        {
            DestroyChameleon();
        }

        public void DestroyChameleon()
        {
            animTime = 0;

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
            

            if (fxController.emitters[0].fxPS != null && fxController.emitters[0].fxPS.isPlaying)
            {
                fxController.Stop(0);
            }
        }

        public void OnRespawn(Player p)
        {
            fxController.StopAndDestroy(0, 0f);
        }

        private void SwapToDamagedModels()
        {
            ToggleSinking(true);
            throw new NotImplementedException();
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
    }
}
