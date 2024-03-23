using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class Camo_Button : MonoBehaviour
    {
        public MeshRenderer subExteriorRenderer;

        public Material subDefaultMaterial;
        public Material camoMaterial;
        public Material accentMaterial;
        public Material canopyMaterial;

        private Color originalCanopyColor;

        private void Start()
        {
            originalCanopyColor = canopyMaterial.color;
        }

        public void EnableCamo()
        {
            subExteriorRenderer.materials[0] = camoMaterial;
            accentMaterial.EnableKeyword("_Emission");
            canopyMaterial.color = new Color(108, 108, 108, 255);
        }

        public void DisableCamo()
        {
            subExteriorRenderer.materials[0] = subDefaultMaterial;
            accentMaterial.DisableKeyword("_Emission");
            canopyMaterial.color = originalCanopyColor;
        }
    }
}
