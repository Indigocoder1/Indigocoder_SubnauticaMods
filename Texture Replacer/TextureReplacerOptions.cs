using Nautilus.Handlers;
using Nautilus.Options;

namespace TextureReplacer
{
    internal class TextureReplacerOptions : ModOptions
    {
        public TextureReplacerOptions() : base("Texture Replacer Options")
        {
            var writeLogsOption = Main.WriteLogs.ToModToggleOption();
            writeLogsOption.OnChanged += OnWriteLogsChanged;
            AddItem(writeLogsOption);

            OptionsPanelHandler.RegisterModOptions(this);
        }

        private void OnWriteLogsChanged(object sender, ToggleChangedEventArgs e)
        {
            Main.WriteLogs.Value = e.Value;
        }
    }
}
