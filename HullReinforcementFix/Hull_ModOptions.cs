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

            var mk1DamageReductionOption = Main_Plugin.MK1DamageReductionMultiplier.ToModSliderOption(step: 0.1f, floatFormat: "{0:F1}");
            mk1DamageReductionOption.OnChanged += MK1DamageReductionOption_OnChanged;
            AddItem(mk1DamageReductionOption);

            var mk2DamageReductionOption = Main_Plugin.MK2DamageReductionMultiplier.ToModSliderOption(step: 0.1f, floatFormat: "{0:F1}");
            mk2DamageReductionOption.OnChanged += MK2DamageReductionOption_OnChanged;
            AddItem(mk2DamageReductionOption);

            var mk3DamageReductionOption = Main_Plugin.MK3DamageReductionMultiplier.ToModSliderOption(step: 0.1f, floatFormat: "{0:F1}");
            mk3DamageReductionOption.OnChanged += MK3DamageReductionOption_OnChanged;
            AddItem(mk3DamageReductionOption);

            OptionsPanelHandler.RegisterModOptions(this);
        }

        private void WriteLogsOption_OnChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.WriteLogs.Value = e.Value;
        }
        private void MK1DamageReductionOption_OnChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.MK1DamageReductionMultiplier.Value = e.Value;
        }
        private void MK2DamageReductionOption_OnChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.MK2DamageReductionMultiplier.Value = e.Value;
        }
        private void MK3DamageReductionOption_OnChanged(object sender, SliderChangedEventArgs e)
        {
            Main_Plugin.MK3DamageReductionMultiplier.Value = e.Value;
        }
    }
}
