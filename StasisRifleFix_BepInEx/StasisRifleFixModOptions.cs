using Nautilus.Handlers;
using Nautilus.Options;

namespace StasisRifleFixMod_BepInEx
{
    public class StasisRifleFixModOptions : ModOptions
    {
        public StasisRifleFixModOptions() : base("Stasis Rifle Fix Options")
        {
            var writeLogsOption = Main_Plugin.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += OnWriteLogsChanged;
            AddItem(writeLogsOption);

            OptionsPanelHandler.RegisterModOptions(this);
        }

        private void OnWriteLogsChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.WriteLogs.Value = e.Value;
        }
    }
}
