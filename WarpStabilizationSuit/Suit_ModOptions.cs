using Nautilus.Handlers;
using Nautilus.Options;

namespace WarpStabilizationSuit
{
    internal class Suit_ModOptions : ModOptions
    {
        public Suit_ModOptions() : base("Warp Stabilization Suit Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            var harderRecipeOption = Main_Plugin.UseHardRecipe.ToModToggleOption();
            harderRecipeOption.OnChanged += OnHarderRecipeOptionChanged; ;
            AddItem(harderRecipeOption);
        }

        private void OnHarderRecipeOptionChanged(object sender, ToggleChangedEventArgs e)
        {
            Main_Plugin.UseHardRecipe.Value = e.Value;
        }
    }
}
