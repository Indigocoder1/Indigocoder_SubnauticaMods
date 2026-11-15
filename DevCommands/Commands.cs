using Nautilus.Commands;

namespace DevCommands
{
    public static class Commands
    {
        [ConsoleCommand("devGamemode")]
        public static string SetDevGamemodeCommand()
        {
            DevConsole.SendConsoleCommand("freedom");
            GameModeUtils.SetGameMode(GameModeOption.NoOxygen | GameModeOption.NoRadiation | GameModeOption.NoAggression | GameModeOption.NoCost 
                , GameModeOption.None);
            DevConsole.SendConsoleCommand("fastscan");
            DevConsole.SendConsoleCommand("fastbuild");
            return "";
        }

        [ConsoleCommand("devItems")]
        public static string DevItemsCommand()
        {
            DevConsole.SendConsoleCommand("item seaglide");
            DevConsole.SendConsoleCommand("item scanner");
            DevConsole.SendConsoleCommand("item builder");
            DevConsole.SendConsoleCommand("item highcapacitytank");

            return "";
        }

        [ConsoleCommand("headlamp")]
        public static string HeadlampCommand()
        {
            PlayerPatch.ToggleLightActive();

            return "";
        }

        [ConsoleCommand("visualizebatches")]
        public static string VisualizeBatchesCommand()
        {
            PlayerPatch.ToggleBatchVisualization();

            return "";
        }
    }
}
