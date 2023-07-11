using Nautilus.Handlers;
using Nautilus.Options;

namespace StasisRifleFixMod_BepInEx
{
    public class StasisRifleFixModOptions : ModOptions
    {
        public static ModToggleOption WriteLogs;

        public StasisRifleFixModOptions() : base("Stasis Rifle Fix Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            WriteLogs = ModToggleOption.Create("writeLogs", "Write Logs", false);
            AddItem(WriteLogs);
        }
    }
}
