using BepInEx.Configuration;
using Nautilus.Handlers;
using Nautilus.Options;

namespace GrappleItemPickup_BepInEx
{
    public class GrappleItemPickupModOptions : ModOptions
    {
        private ConfigFile configFile;
        private ModToggleOption writeLogsOption;
        private ModSliderOption pickupDistanceOption;

        public GrappleItemPickupModOptions() : base("Grapple Item Pickup Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            writeLogsOption = GrappleItemPickupPlugin.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += OnWriteLogsChanged;
            AddItem(writeLogsOption);

            pickupDistanceOption = GrappleItemPickupPlugin.PickupDistance.ToModSliderOption();
            pickupDistanceOption.OnChanged += OnPickupDistanceChanged;
            AddItem(pickupDistanceOption);
        }

        private void OnWriteLogsChanged(object sender, OptionEventArgs e)
        {
            GrappleItemPickupPlugin.WriteLogs.Value = writeLogsOption.Value;;
        }

        private void OnPickupDistanceChanged(object sender, SliderChangedEventArgs e)
        {
            GrappleItemPickupPlugin.PickupDistance.Value = pickupDistanceOption.Value;
        }
    }
}