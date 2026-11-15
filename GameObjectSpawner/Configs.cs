using Nautilus.Handlers;
using Nautilus.Options;

namespace GameObjectSpawner
{
    internal class Configs : ModOptions
    {
        public Configs() : base("GameObject Spawner")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            /*
            var spawnKeyOption = Plugin.spawnKey.ToModKeybindOption();
            spawnKeyOption.OnChanged += (s, e) => Plugin.spawnKey.Value = e.Value;
            AddItem(spawnKeyOption);

            var spawnDistOption = Plugin.spawnDistance.ToModSliderOption(0, 50);
            spawnDistOption.OnChanged += (s, e) => Plugin.spawnDistance.Value = (int)e.Value;
            AddItem(spawnDistOption);

            var spawnScaleOption = Plugin.spawnScaleMultiplier.ToModSliderOption(minValue: 0f, maxValue: 10f, step: 0.1f, floatFormat: "{0:F1}");
            spawnScaleOption.OnChanged += (s, e) => Plugin.spawnScaleMultiplier.Value = e.Value;
            AddItem(spawnScaleOption);
            */
        }
    }
}
