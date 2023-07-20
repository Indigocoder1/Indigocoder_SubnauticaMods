using Nautilus.Handlers;
using Nautilus.Options;

namespace UpgradedJumpJetModule
{
    internal class UpgradedJumpJet_ModOptions : ModOptions
    {
        public UpgradedJumpJet_ModOptions() : base ("Upgraded Jump Jet Mod Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var upgradedJetForce = Main_Plugin.UpgradedJetForce.ToModSliderOption(step: 1f, floatFormat: "{0}");
            upgradedJetForce.OnChanged += OnUpgradedJetForceChanged;
            AddItem(upgradedJetForce);

            var upgradedJetAcceleration = Main_Plugin.UpgradedJetAcceleration.ToModSliderOption(step: 0.1f, floatFormat: "{0:F1}");
            upgradedJetAcceleration.OnChanged += OnUpgradedJetForceChanged;
            AddItem(upgradedJetAcceleration);

            var upgradedJumpForce = Main_Plugin.UpgradedJumpForce.ToModSliderOption(step: 1f, floatFormat: "{0}");
            upgradedJumpForce.OnChanged += OnUpgradedJumpForceChanged;
            AddItem(upgradedJumpForce);
        }

        private void OnUpgradedJetForceChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.UpgradedJetAcceleration.Value = e.Value;
        }
        private void OnUpgradedJetAccelerationChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.UpgradedJetForce.Value = e.Value;
        }

        private void OnUpgradedJumpForceChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.UpgradedJetAcceleration.Value = e.Value;
        }
    }
}
