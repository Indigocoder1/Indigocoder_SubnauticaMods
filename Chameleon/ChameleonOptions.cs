using Nautilus.Handlers;
using Nautilus.Options;

namespace Chameleon
{
    public class ChameleonOptions : ModOptions
    {
        public ChameleonOptions() : base("Chameleon Mod Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var cloakEffect = Main_Plugin.UseLegacyCloakEffect.ToModToggleOption();
            cloakEffect.OnChanged += (object _, ToggleChangedEventArgs args) => Main_Plugin.UseLegacyCloakEffect.Value = args.Value;
            AddItem(cloakEffect);
        }
    }
}
