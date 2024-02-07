using UnityEngine;

namespace VariableGravityCannon.Monos
{
    internal class VariableGravityCannon_Mono : MonoBehaviour
    {
        [SerializeField] private bool isPropActive;
        [SerializeField] private RepulsionCannon repulsionCannon;
        [SerializeField] private PropulsionCannonWeapon propulsionCannonWeapon;
        [SerializeField] private Renderer renderer;

        [SerializeField] private EnergyMixin energyMixin;
        [SerializeField] private PropulsionCannon propulsionCannon;

        public void Initialize(RepulsionCannon repulsionCannon, PropulsionCannonWeapon propulsionCannon, Renderer renderer)
        {
            this.repulsionCannon = repulsionCannon;
            this.propulsionCannonWeapon = propulsionCannon;
            this.renderer = renderer;

            energyMixin = GetComponent<EnergyMixin>();
            this.propulsionCannon = GetComponent<PropulsionCannon>();
        }

        public void ToggleCannonType()
        {
            repulsionCannon.enabled = isPropActive;
            propulsionCannonWeapon.enabled = !isPropActive;

            if(isPropActive)
            {
                repulsionCannon.OnDraw(propulsionCannonWeapon.usingPlayer);
                propulsionCannon.ReleaseGrabbedObject();
            }

            isPropActive = !isPropActive;

            renderer.material.SetTexture("_MainTex", isPropActive ? Main_Plugin.variablePropTex : Main_Plugin.variableRepulsionTex);
            renderer.material.SetTexture("_SpecTex", isPropActive ? Main_Plugin.variablePropTex : Main_Plugin.variableRepulsionTex);

            renderer.material.SetTexture("_Illum", isPropActive ? Main_Plugin.variablePropIllum : Main_Plugin.variableRepulsionIllum);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;
            if (IngameMenu.main.selected) return;

            string subscript = $"Power: {Mathf.Floor(energyMixin.GetBatteryChargeValue() * 100)}%\n" +
                $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Toggle Cannon Mode", uGUI.FormatButton(GameInput.Button.Deconstruct))}";
            HandReticle.main.textUseSubscript = subscript;
        }

        public bool IsPropActive()
        {
            return isPropActive;
        }

        public PlayerTool GetActiveCannon()
        {
            return isPropActive ? propulsionCannonWeapon : repulsionCannon;
        }
    }
}
