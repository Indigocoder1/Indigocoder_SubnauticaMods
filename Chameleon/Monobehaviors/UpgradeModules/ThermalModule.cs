using Chameleon.Attributes;
using UnityEngine;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    [ChameleonUpgradeModule("SealThermalChargeModule")]
    internal class ThermalModule : BaseChargerModule<ThermalChargerFunction>
    {
        
    }

    internal class ThermalChargerFunction : BaseChargerFunction
    {
        private AnimationCurve thermalCharge;

        public override void Awake()
        {
            base.Awake();
            thermalCharge = GetComponentInParent<SubRoot>().thermalReactorCharge;
        }

        public override float GetCharge()
        {
            float temp = GetTemperature();
            return thermalCharge.Evaluate(temp) * 1.5f * Time.deltaTime;
        }

        private float GetTemperature()
        {
            WaterTemperatureSimulation main = WaterTemperatureSimulation.main;
            if (!main) return 0f;

            return main.GetTemperature(transform.position);
        }
    }
}
