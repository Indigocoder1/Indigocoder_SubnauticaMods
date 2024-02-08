using Nautilus.Handlers;
using Nautilus.Options;
using System;
using System.Collections.Generic;

namespace ImprovedGravTrap
{
    internal class GravTrap_ModOptions : ModOptions
    {
        public static event EventHandler OnStorageSizeChange;

        public GravTrap_ModOptions() : base("Improved Grav Trap Mod Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var useScrollWheelOption = Main_Plugin.UseScrollWheel.ToModToggleOption();
            useScrollWheelOption.OnChanged += OnUseScrollWheelOptionChanged;
            AddItem(useScrollWheelOption);

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

            var storageWidthOption = Main_Plugin.GravTrapStorageWidth.ToModSliderOption();
            storageWidthOption.OnChanged += OnStorageWidthOptionChanged;
            AddItem(storageWidthOption);

            var storageHeightOption = Main_Plugin.GravTrapStorageHeight.ToModSliderOption();
            storageHeightOption.OnChanged += OnStorageHeightOptionChanged;
            AddItem(storageHeightOption);

            var storageOpenDistOption = Main_Plugin.GravStorageOpenDistance.ToModSliderOption(minValue: 2f, maxValue: 6f, step: 0.5f, floatFormat: "{0:F1}");
            storageOpenDistOption.OnChanged += OnStorageOpenDistOptionChanged;
            AddItem(storageOpenDistOption);

            var storagePickupDistance = Main_Plugin.GravStoragePickupDistance.ToModSliderOption(minValue: 2f, maxValue: 10f, step: 1f, floatFormat: "{0}");
            storagePickupDistance.OnChanged += OnStoragePickupDistanceChanged;
            AddItem(storagePickupDistance);

            List<PingType> pingTypes = new List<PingType> { PingType.Lifepod, PingType.Seamoth,
                PingType.Cyclops, PingType.Exosuit, PingType.Rocket, PingType.Beacon, PingType.Signal, PingType.MapRoomCamera, PingType.Sunbeam, PingType.ControlRoom};
            var beaconType = Main_Plugin.GravTrapBeaconType.ToModChoiceOption(options: pingTypes);
            beaconType.OnChanged += OnBeaconTypeChanged;
            AddItem(beaconType);
        }

        private void OnUseScrollWheelOptionChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.UseScrollWheel.Value = e.Value;
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
        private void OnStorageWidthOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.GravTrapStorageWidth.Value = (int)e.Value;
            OnStorageSizeChange?.Invoke(this, EventArgs.Empty);
        }
        private void OnStorageHeightOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.GravTrapStorageHeight.Value = (int)e.Value;
            OnStorageSizeChange?.Invoke(this, EventArgs.Empty);
        }
        private void OnStorageOpenDistOptionChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.GravStorageOpenDistance.Value = e.Value;
        }
        private void OnStoragePickupDistanceChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.GravStoragePickupDistance.Value = e.Value;
        }
        private void OnBeaconTypeChanged(object sender, ChoiceChangedEventArgs<PingType> e)
        {
            Main_Plugin.GravTrapBeaconType.Value = e.Value;
        }
    }
}
