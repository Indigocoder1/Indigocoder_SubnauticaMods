using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Handlers.TitleScreen;

namespace DevCommands
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.DevCommands";
        private const string pluginName = "Dev Commands";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
        {
            logger = Logger;

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(Commands));

            harmony.PatchAll();
            
            TitleScreenHandler.ApproveTitleCollaboration(this,
                new TitleScreenHandler.CollaborationData(new[] { "com.prototech.prototypesub" }));

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }
    }
}
