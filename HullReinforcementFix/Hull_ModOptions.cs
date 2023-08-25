using Nautilus.Handlers;
using Nautilus.Options;

namespace HullReinforcementFix
{
    internal class Hull_ModOptions : ModOptions
    {
        public Hull_ModOptions() : base("Hull Reinforcement Fix Options")
        {
            var writeLogsOption = Main_Plugin.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += WriteLogsOption_OnChanged; ;
            AddItem(writeLogsOption);

            var damageReductionOption = Main_Plugin.DamageReductionMultiplier.ToModSliderOption(step: 0.1f, floatFormat: "{0:F1}");
            damageReductionOption.OnChanged += DamageReductionOption_OnChanged;
            AddItem(damageReductionOption);

            OptionsPanelHandler.RegisterModOptions(this);
        }

        private void WriteLogsOption_OnChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.WriteLogs.Value = e.Value;
        }
        private void DamageReductionOption_OnChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.DamageReductionMultiplier.Value = e.Value;
        }
    }
}
