using Nautilus.Handlers;
using Nautilus.Options;

namespace ImprovedGravTrap
{
    internal class Options : ModOptions
    {
        public Options() : base("Variable Gravity Cannon Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var repulsionFireEnergy = Main_Plugin.RepulsionModeFireEnergy.ToModSliderOption(floatFormat: "{0:F0}", step: 1f);
            repulsionFireEnergy.OnChanged += (sender, e) => { Main_Plugin.RepulsionModeFireEnergy.Value = e.Value; };
            AddItem(repulsionFireEnergy);

            var propulsionFireEnergy = Main_Plugin.PropulsionModeFireEnergy.ToModSliderOption(floatFormat: "{0:F0}", step: 1f);
            propulsionFireEnergy.OnChanged += (sender, e) => { Main_Plugin.PropulsionModeFireEnergy.Value = e.Value; };
            AddItem(propulsionFireEnergy);

            var propulsionConstEnergy = Main_Plugin.PropulsionModePerSecondEnergy.ToModSliderOption(floatFormat: "{0:F1}", step: 0.1f);
            propulsionConstEnergy.OnChanged += (sender, e) => { Main_Plugin.PropulsionModePerSecondEnergy.Value = e.Value; };
            AddItem(propulsionConstEnergy);
        }
    }
}
