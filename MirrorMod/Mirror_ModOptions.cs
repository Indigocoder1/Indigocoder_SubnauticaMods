using Nautilus.Handlers;
using Nautilus.Options;

namespace MirrorMod
{
    internal class Mirror_ModOptions : ModOptions
    {
        public Mirror_ModOptions() : base("Mirror Mod Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var resolutionModOption = Main_Plugin.MirrorResolution.ToModSliderOption(step: 50);
            resolutionModOption.OnChanged += OnMirrorResolutionModChanged;
            AddItem(resolutionModOption);
        }

        private void OnMirrorResolutionModChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.MirrorResolution.Value = (int)e.Value;
        }
    }
}
