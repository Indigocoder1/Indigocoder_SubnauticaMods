using Nautilus.Handlers;
using Nautilus.Options;

namespace StasisRifleFixMod_BepInEx
{
    public class StasisRifleFixModOptions : ModOptions
    {
        private ModToggleOption writeLogsOption;

        public StasisRifleFixModOptions() : base("Stasis Rifle Fix Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            writeLogsOption = StasisFreezeFixPlugin.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += OnWriteLogsChanged;
            AddItem(writeLogsOption);
        }

        private void OnWriteLogsChanged(object sender, OptionEventArgs e)
        {
            StasisFreezeFixPlugin.WriteLogs.Value = writeLogsOption.Value; ;
        }
    }
}
