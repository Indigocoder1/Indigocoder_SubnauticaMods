using Nautilus.Handlers;
using Nautilus.Options;

namespace VariableGravityCannon
{
    internal class Options : ModOptions
    {
        public Options() : base("Variable Gravity Cannon Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var repulsionFireEnergy = Main_Plugin.RepulsionModeFireEnergy.ToModSliderOption();
            repulsionFireEnergy.OnChanged += (sender, e) => { Main_Plugin.RepulsionModeFireEnergy.Value = e.Value; };
            AddItem(repulsionFireEnergy);

            var propulsionFireEnergy = Main_Plugin.PropulsionModeFireEnergy.ToModSliderOption();
            propulsionFireEnergy.OnChanged += (sender, e) => { Main_Plugin.PropulsionModeFireEnergy.Value = e.Value; };
            AddItem(propulsionFireEnergy);

            var propulsionConstEnergy = Main_Plugin.PropulsionModePerSecondEnergy.ToModSliderOption();
            propulsionConstEnergy.OnChanged += (sender, e) => { Main_Plugin.PropulsionModePerSecondEnergy.Value = e.Value; };
            AddItem(propulsionConstEnergy);
        }
    }
}
