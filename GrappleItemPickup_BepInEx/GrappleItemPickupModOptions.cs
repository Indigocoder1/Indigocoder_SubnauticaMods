using BepInEx.Configuration;
using Nautilus.Handlers;
using Nautilus.Options;

namespace GrappleItemPickup_BepInEx
{
    public class GrappleItemPickupModOptions : ModOptions
    {
        private ModToggleOption enableModOption;
        private ModToggleOption writeLogsOption;
        private ModSliderOption pickupDistanceOption;

        public GrappleItemPickupModOptions() : base("Grapple Item Pickup Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            enableModOption = GrappleItemPickupPlugin.EnableMod.ToModToggleOption();
            enableModOption.OnChanged += OnEnableModChanged;
            AddItem(enableModOption);

            writeLogsOption = GrappleItemPickupPlugin.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += OnWriteLogsChanged;
            AddItem(writeLogsOption);

            pickupDistanceOption = GrappleItemPickupPlugin.PickupDistance.ToModSliderOption();
            pickupDistanceOption.OnChanged += OnPickupDistanceChanged;
            AddItem(pickupDistanceOption);
        }

        private void OnEnableModChanged(object sender, OptionEventArgs e)
        {
            GrappleItemPickupPlugin.EnableMod.Value = enableModOption.Value;
        }

        private void OnWriteLogsChanged(object sender, OptionEventArgs e)
        {
            GrappleItemPickupPlugin.WriteLogs.Value = writeLogsOption.Value;
        }
        private void OnPickupDistanceChanged(object sender, SliderChangedEventArgs e)
        {
            GrappleItemPickupPlugin.PickupDistance.Value = pickupDistanceOption.Value;
        }
    }
}