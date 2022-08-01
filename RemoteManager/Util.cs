using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace RemoteManager
{
    public static class Util
    {
        public static PropertyInfo commandProperty = typeof(CommandEventArgs).GetProperty("Command");

        public static void WriteLine(string text, ConsoleColor color)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }

        public static TSPlayer[] GetOnlinePlayers()
        {
            List<TSPlayer> players = new List<TSPlayer>();
            foreach (var player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    players.Add(player);
                }
            }
            return players.ToArray();
        }

        public static string GetCommandArgs(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length)
                    {
                        return args[i + 1];
                    }
                }
            }
            return null;
        }

        public static void SendCommand(string command)
        {
            CommandEventArgs args = new CommandEventArgs();
            commandProperty.SetValue(args, command);
            ServerApi.Hooks.ServerCommand.Invoke(args);
        }
    }
}
