using Nautilus.Handlers;
using Nautilus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedGravTrap
{
    internal class GravTrap_ModOptions : ModOptions
    {
        public GravTrap_ModOptions() : base("Improved Grav Trap Mod Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var useScrollWheelOption = Main_Plugin.UseScrollWheel.ToModToggleOption();
            useScrollWheelOption.OnChanged += OnUseScrollWheelOptionChanged;
            AddItem(useScrollWheelOption);

            var keycodeOption = Main_Plugin.AdvanceKey.ToModKeybindOption();
            keycodeOption.OnChanged += OnKeycodeOptionChanged;
            AddItem(keycodeOption);

            var rangeOption = Main_Plugin.EnhancedRange.ToModSliderOption(minValue: 17, maxValue: 40, step: 1);
            rangeOption.OnChanged += OnRangeOptionChanged;
            AddItem(rangeOption);

            var forceOption = Main_Plugin.EnhancedMaxForce.ToModSliderOption(minValue: 15f, maxValue: 30f, 1f, floatFormat: "{0}");
            forceOption.OnChanged += OnForceOptionChanged;
            AddItem(forceOption);

            var massOption = Main_Plugin.EnhancedMaxMassStable.ToModSliderOption(minValue: 15f, maxValue: 200f, step: 1f, floatFormat: "{0}");
            massOption.OnChanged += OnMassOptionChanged;
            AddItem(massOption);

            var maxAmountOption = Main_Plugin.EnhancedMaxObjects.ToModSliderOption(minValue: 12, maxValue: 30, step: 1);
            maxAmountOption.OnChanged += OnMaxAmountOptionChanged;
            AddItem(maxAmountOption);
        }

        private void OnUseScrollWheelOptionChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.UseScrollWheel.Value = e.Value;
        }
        private void OnKeycodeOptionChanged(object sender, KeybindChangedEventArgs e)
        {
            Main_Plugin.AdvanceKey.Value = e.Value;
        }
        private void OnRangeOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.EnhancedRange.Value = (int)e.Value;
        }
        private void OnForceOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.EnhancedMaxForce.Value = e.Value;
        }
        private void OnMassOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.EnhancedMaxMassStable.Value = e.Value;
        }
        private void OnMaxAmountOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.EnhancedMaxObjects.Value = (int)e.Value;
        }
    }
}
