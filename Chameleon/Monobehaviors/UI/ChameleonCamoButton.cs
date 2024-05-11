using Chameleon.ScriptableObjects;
using UnityEngine;

namespace Chameleon.Monobehaviors.UI
{
    public class ChameleonCamoButton : MonoBehaviour
    {
        [Header("Sub References")]
        public SubRoot subRoot;
        public Animator animator;
        public ToggleLights toggleLights;

        [Header("Mesh Renderers")]
        public MeshRenderer subExteriorRenderer;
        public MeshRenderer conningTowerRenderer;
        public MeshRenderer canopyRenderer;

        public GameObject[] interiorRendererParents;

        [Header("Entry Hatches")]
        public SubEnterHandTarget entryHatch;
        public SubEnterHandTarget exitHatch;

        [Header("Materials")]
        public Material camoMaterial;

        [Header("Power Consumption")]
        public float timeBetweenDischarges;
        public float energyCostPerDischarge;

        [Header("Miscellaneous")]
        public float glowSpeed;

        [Header("Audio")]
        public FMOD_CustomLoopingEmitter loopingSFX;
        public VoiceNotification camoActivateSFX;
        public VoiceNotification camoDeactivateSFX;

        private Material subDefaultMaterial;
        private Material subAccentMaterial;
        private Material conningTowerAccentMaterial;
        private Material canopyMaterial;

        private Color originalCanopyColor;

        private bool isActive;
        private bool hovering;
        private bool previousLightsState;
        private float glowWaverTime;

        private void Start()
        {
            subDefaultMaterial = subRoot.transform.Find("Model/Exterior/Sub_Body").GetComponent<Renderer>().materials[0];

            conningTowerAccentMaterial = subRoot.transform.Find("Model/Exterior/Sub_ConningTower").GetComponent<Renderer>().materials[1];
            subAccentMaterial = subRoot.transform.Find("Model/Exterior/Sub_Body").GetComponent<Renderer>().materials[1];
            canopyMaterial = subRoot.transform.Find("Model/Exterior/Sub_Canopy").GetComponent<Renderer>().materials[0];
            originalCanopyColor = canopyMaterial.color;

            exitHatch.OnEnter = OnExitSub;
            entryHatch.OnEnter = OnEnterSub;
        }

        private void Update()
        {
            if(hovering)
            {
                HandReticle main = HandReticle.main;
                main.SetText(HandReticle.TextType.Hand, "ChameleonCamoButton", true, GameInput.Button.LeftHand);
                main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            }
        }

        public void OnMouseEnter()
        {
            if (Player.main.currentSub != subRoot) return;

            hovering = true;
            
        }

        public void OnMouseExit()
        {
            if (Player.main.currentSub != subRoot) return;

            HandReticle.main.SetIcon(HandReticle.IconType.Default, 1f);
            hovering = false;
        }
        
        public void ToggleCamo()
        {
            isActive = !isActive;
            animator.SetBool("CamoActive", isActive);

            if(isActive)
            {
                subRoot.voiceNotificationManager.PlayVoiceNotification(camoActivateSFX);
                loopingSFX.Play();

                InvokeRepeating(nameof(DrawPowerRepeating), 0, timeBetweenDischarges);
            }
            else
            {
                CancelInvoke(nameof(DrawPowerRepeating));
                DisableCamoEffect();

                subRoot.voiceNotificationManager.PlayVoiceNotification(camoDeactivateSFX);
                loopingSFX.Stop();
            }
        }

        public void OnExitSub()
        {
            if (canopyMaterial == null || !isActive) return;

            EnableCamoEffect();
            DisableInterior();            
        }

        public void OnEnterSub()
        {
            if (canopyMaterial == null || !isActive) return;

            DisableCamoEffect();
            EnableInterior();
        }

        public void EnableCamoEffect()
        {
            if (!isActive) return;

            Material[] newBodyMaterials = subExteriorRenderer.materials;
            Material[] newTowerMaterials = conningTowerRenderer.materials;
            newTowerMaterials[0].SetInt("_ZWrite", 0);

            newBodyMaterials[0] = camoMaterial;
            newBodyMaterials[1] = camoMaterial;

            newTowerMaterials[0] = camoMaterial;
            newTowerMaterials[1] = camoMaterial;

            subExteriorRenderer.materials = newBodyMaterials;
            conningTowerRenderer.materials = newTowerMaterials;

            conningTowerAccentMaterial.EnableKeyword("MARMO_EMISSION");
            subAccentMaterial.EnableKeyword("MARMO_EMISSION");
        }

        public void DisableCamoEffect()
        {
            Material[] newBodyMaterials = subExteriorRenderer.materials;
            Material[] newTowerMaterials = conningTowerRenderer.materials;

            newBodyMaterials[0] = subDefaultMaterial;
            newBodyMaterials[1] = subAccentMaterial;

            newTowerMaterials[0] = subDefaultMaterial;
            newTowerMaterials[1] = subAccentMaterial;

            subExteriorRenderer.materials = newBodyMaterials;
            conningTowerRenderer.materials = newTowerMaterials;

            conningTowerAccentMaterial.DisableKeyword("MARMO_EMISSION");
            subAccentMaterial.DisableKeyword("MARMO_EMISSION");
        }

        public void EnableInterior()
        {
            if (!isActive) return;

            for (int i = 0; i < interiorRendererParents.Length; i++)
            {
                foreach (Renderer rend in interiorRendererParents[i].GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = true;
                }
            }

            canopyRenderer.material = canopyMaterial;

            toggleLights.SetLightsActive(previousLightsState);

            if (Player.main.GetCurrentSub() == subRoot)
            {
                foreach (Renderer rend in Player.main.transform.Find("body/player_view/male_geo").GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = true;
                }
            }
        }

        public void DisableInterior()
        {
            if (!isActive) return;

            for (int i = 0; i < interiorRendererParents.Length; i++)
            {
                foreach (Renderer rend in interiorRendererParents[i].GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = false;
                }
            }

            canopyRenderer.material = camoMaterial;

            previousLightsState = toggleLights.lightsActive;
            toggleLights.SetLightsActive(false);

            if (Player.main.GetCurrentSub() == subRoot)
            {
                foreach (Renderer rend in Player.main.transform.Find("body/player_view").GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = false;
                }
            }
        }

        public bool IsCamoActive()
        {
            return isActive;
        }

        private void DrawPowerRepeating()
        {
            if (!subRoot.powerRelay.ConsumeEnergy(energyCostPerDischarge, out float amountConsumed))
            {
                DisableCamoEffect();
            }
        }

        private void OnDestroy()
        {
            exitHatch.OnEnter -= OnExitSub;
            entryHatch.OnEnter -= OnEnterSub;
        }
    }
}
