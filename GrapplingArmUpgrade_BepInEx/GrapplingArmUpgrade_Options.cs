using Nautilus.Handlers;
using Nautilus.Options;

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

            var hookSpeedOption = Main_Plugin.InitialHookSpeed.ToModSliderOption(step: 1f, floatFormat: "{0}");
            hookSpeedOption.OnChanged += OnHookSpeedChanged;
            AddItem(hookSpeedOption);

            var exosuitAccelerationOption = Main_Plugin.ExosuitAcceleration.ToModSliderOption(step: 1f, floatFormat: "{0}");
            exosuitAccelerationOption.OnChanged += OnExosuitAccelerationChanged;
            AddItem(exosuitAccelerationOption);

            var attachedObjectAcceleration = Main_Plugin.AttachedObjectAcceleration.ToModSliderOption(step: 1f, floatFormat: "{0}");
            attachedObjectAcceleration.OnChanged += OnAttachedObjectAccelerationChanged;
            AddItem(attachedObjectAcceleration);
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
            Main_Plugin.InitialHookSpeed.Value = e.Value;
        }
        private void OnExosuitAccelerationChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.ExosuitAcceleration.Value = e.Value;
        }
        private void OnAttachedObjectAccelerationChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.AttachedObjectAcceleration.Value = e.Value;
        }
    }
}
