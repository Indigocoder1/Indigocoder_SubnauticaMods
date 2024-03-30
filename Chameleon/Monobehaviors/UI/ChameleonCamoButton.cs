using System;
using UnityEngine;

namespace Chameleon.Monobehaviors.UI
{
    internal class ChameleonCamoButton : MonoBehaviour
    {
        public SubRoot subRoot;

        public MeshRenderer subExteriorRenderer;
        public GameObject interiorModelParent;
        public SubEnterHandTarget entryHatch;
        public SubEnterHandTarget exitHatch;

        public Material subDefaultMaterial;
        public Material camoMaterial;
        public Material accentMaterial;
        public Material canopyMaterial;

        private Color originalCanopyColor;
        private bool active;

        private void Start()
        {
            originalCanopyColor = canopyMaterial.color;
            exitHatch.OnEnter += DisableInterior;
            entryHatch.OnEnter += EnableInterior;
        }

        public void OnMouseEnter()
        {
            if (Player.main.currentSub != subRoot) return;

            HandReticle main = HandReticle.main;
            main.SetText(HandReticle.TextType.Hand, "ChameleonCamoButton", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        }

        public void OnMouseExit()
        {
            if (Player.main.currentSub != subRoot) return;

            HandReticle.main.SetIcon(HandReticle.IconType.Default, 1f);
        }

        public void ToggleCamo()
        {
            active = !active;

            if(active)
            {
                subExteriorRenderer.materials[0] = camoMaterial;
                accentMaterial.EnableKeyword("_Emission");
            }
            else
            {
                subExteriorRenderer.materials[0] = subDefaultMaterial;
                accentMaterial.DisableKeyword("_Emission");
            }
        }

        private void DisableInterior(object sender, EventArgs e)
        {
            interiorModelParent.SetActive(false);

            canopyMaterial.color = new Color(108 / 255f, 108 / 255f, 108 / 255f, 1f);
        }

        private void EnableInterior(object sender, EventArgs e)
        {
            interiorModelParent.SetActive(true);
            canopyMaterial.color = originalCanopyColor;
        }

        private void OnDestroy()
        {
            exitHatch.OnEnter -= DisableInterior;
            entryHatch.OnEnter -= EnableInterior;
        }
    }
}
