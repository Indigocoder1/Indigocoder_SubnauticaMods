using BepInEx.Configuration;
using Nautilus.Handlers;
using Nautilus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class GrapplingArmUpgrade_Options : ModOptions
    {
        public GrapplingArmUpgrade_Options() : base("Grappling Arm Upgrade Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var enableModOption = Main_Plugin.EnableMod.ToModToggleOption();
            enableModOption.OnChanged += OnEnableModChanged;
            AddItem(enableModOption);

            var armCooldownOption = Main_Plugin.ArmCooldown.ToModSliderOption(step: 0.1f, floatFormat: "{0:F1}");
            armCooldownOption.OnChanged += OnArmCooldownChanged;
            AddItem(armCooldownOption);

            var hookMaxDistanceOption = Main_Plugin.HookMaxDistance.ToModSliderOption(step: 1f, floatFormat: "{0}");
            hookMaxDistanceOption.OnChanged += OnHookMaxDistanceChanged;
            AddItem(hookMaxDistanceOption);

            var hookSpeedOption = Main_Plugin.HookSpeed.ToModSliderOption(step: 1f, floatFormat: "{0}");
            hookSpeedOption.OnChanged += OnHookSpeedChanged;
            AddItem(hookSpeedOption);

            var exosuitAccelerationOption = Main_Plugin.ExosuitAcceleration.ToModSliderOption(step: 1f, floatFormat: "{0}");
            exosuitAccelerationOption.OnChanged += OnExosuitAccelerationChanged;
            AddItem(exosuitAccelerationOption);

            var hookShootForceOption = Main_Plugin.HookShootForce.ToModSliderOption(step: 1f, floatFormat: "{0}");
            hookShootForceOption.OnChanged += OnHookShootForceChanged;
            AddItem(hookShootForceOption);
        }

        private void OnEnableModChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.EnableMod.Value = e.Value;
        }
        private void OnArmCooldownChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.ArmCooldown.Value = e.Value;
        }
        private void OnHookMaxDistanceChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.HookMaxDistance.Value = e.Value;
        }
        private void OnHookSpeedChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.HookSpeed.Value = e.Value;
        }
        private void OnExosuitAccelerationChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.ExosuitAcceleration.Value = e.Value;
        }
        private void OnHookShootForceChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.HookShootForce.Value = e.Value;
        }
    }
}
