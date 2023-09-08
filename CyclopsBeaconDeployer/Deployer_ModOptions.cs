using Nautilus.Handlers;
using Nautilus.Options;

namespace CyclopsBeaconDeployer
{
    internal class Deployer_ModOptions : ModOptions
    {
        public Deployer_ModOptions() : base("Cyclops Beacon Deployer Options")
        {
            var writeLogsOption = Main_Plugin.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += WriteLogsOption_OnChanged;
            AddItem(writeLogsOption);

            OptionsPanelHandler.RegisterModOptions(this);
        }

        private void WriteLogsOption_OnChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.WriteLogs.Value = e.Value;
        }
    }
}
