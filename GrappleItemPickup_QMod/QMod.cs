using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using System.Reflection;
using Logger = QModManager.Utility.Logger;

namespace GrappleItemPickup
{
    [QModCore]
    public static class QMod
    {
        internal static Config Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();

        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string modName = $"Indigocoder.{assembly.GetName().Name}";
            Logger.Log(Logger.Level.Info, $"Patching {modName}");
            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }

    [Menu("Grapple Item Pickup")]
    public class Config : ConfigFile
    {
        [Toggle("Write QMod logs")]
        public bool writeLogs = false;

        [Slider("Item Pickup Distance", Format = "{0:F2}", Min = 1.0f, Max = 2.0f, DefaultValue = 1.0f, Step = 0.1f)]
        public float pickupDistance = 1f;
    }
}
