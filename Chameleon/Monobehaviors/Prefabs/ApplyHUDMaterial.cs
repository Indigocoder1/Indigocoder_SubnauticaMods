using Chameleon.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class ApplyHUDMaterial : MonoBehaviour, ICyclopsReferencer
    {
        public Mode mode;

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            Material uiMaterial = cyclops.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/EngineOff_Button").GetComponent<Image>().material;
            switch (mode)
            {
                case Mode.SingleRenderer:
                    Renderer renderer = GetComponent<Renderer>();
                    if(!renderer)
                    {
                        Main_Plugin.logger.LogError($"{transform.name} is trying to use SingleRenderer mode but doesn't have a renderer!");
                        return;
                    }

                    renderer.material = uiMaterial;
                    break;
                case Mode.AllChildRenderers:
                    foreach (var rend in GetComponentsInChildren<Renderer>(true))
                    {
                        for (int i = 0; i < rend.materials.Length; i++)
                        {
                            rend.materials[i] = uiMaterial;
                        }
                    }
                    break;
                case Mode.AllChildGraphics:
                    foreach (var graphic in GetComponentsInChildren<Graphic>(true))
                    {
                        graphic.material = uiMaterial;
                    }
                    break;
            }
        }

        public enum Mode
        {
            SingleRenderer,
            AllChildRenderers,
            AllChildGraphics
        }
    }
}
