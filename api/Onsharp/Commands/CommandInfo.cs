using System;
using System.Collections.Generic;
using Onsharp.Entities;

namespace Onsharp.Commands
{
    internal class CommandInfo
    {
        private const int CommandsPerPage = 5;
        private static readonly List<CommandInfo> Commands = new List<CommandInfo>();

        internal static void Reset()
        {
            lock (Commands)
            {
                Commands.Clear();
            }
        }
        
        internal static void PrintCommands(Player player, int page)
        {
            lock (Commands)
            {
                int pages = CountPages();
                if(page > pages) return;
                string header = $"~7~<============~8~[ ~6*~Help ~7~(~e/~{page + "/" + pages}~7~) ~8~]~7~============>";
                player.SendColoredMessage(header);
                int start = CommandsPerPage * (page - 1);
                int end = start + CommandsPerPage - 1;
                for (int i = start; i <= end; i++)
                {
                    if(Commands.Count <= i) break;
                    CommandInfo command = Commands[i];
                    player.SendColoredMessage("~6*~" + command.CommandText + "~7~" + command.Usage.Text + "~8~ : ~7~" + command.Description);
                    foreach (Usage.Parameter parameter in command.Usage.Parameters)
                    {
                        player.SendColoredMessage("    ~e*~" + parameter.Name + " ~8~(~7~" + parameter.Type + "~8~): ~7~" + parameter.Description);
                    }

                    start++;
                    if(start >= end)
                        break;
                }
                
                player.SendColoredMessage(header);
            }
        }

        internal static void RegisterCommand(Command command)
        {
            lock (Commands)
            {
                Commands.Add(new CommandInfo(command.CommandText, command.Description, command.Usage));
            }
        }

        private static int CountPages()
        {
            return (int) Math.Ceiling(Commands.Count / (float) CommandsPerPage);
        }
        
        private string CommandText { get; }
        
        private string Description { get; }
        
        private Usage Usage { get; }

        public CommandInfo(string commandText, string description, Usage usage)
        {
            CommandText = commandText;
            Description = description;
            Usage = usage;
        }
    }
}