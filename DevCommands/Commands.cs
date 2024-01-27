using Nautilus.Commands;

namespace DevCommands
{
    public static class Commands
    {
        [ConsoleCommand("devGamemode")]
        public static string SetDevGamemodeCommand()
        {
            GameModeUtils.SetGameMode(GameModeOption.NoOxygen | GameModeOption.NoRadiation | GameModeOption.NoAggression | GameModeOption.NoCost
                , GameModeOption.None);
            return "";
        }

        [ConsoleCommand("devItems")]
        public static string DevItemsCommand()
        {
            DevConsole.SendConsoleCommand("item seaglide");
            DevConsole.SendConsoleCommand("item scanner");
            DevConsole.SendConsoleCommand("item builder");

            return "";
        }
    }
}
