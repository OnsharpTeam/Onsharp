using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Onsharp.Entities;
using Onsharp.Events;
using Onsharp.IO;
using Onsharp.Native;

namespace Onsharp.Commands
{
    /// <summary>
    /// The command manager takes responsibility for all registered commands and executes them according to the given data.
    /// It also monitors the individual types and forms the parameters according to the handler.
    /// </summary>
    internal class CommandManager
    {
        private readonly List<Command> _commands;
        private readonly Server _server;

        internal CommandManager(Server server = null)
        {
            _server = server;
            _commands = new List<Command>();
        }

        internal void RegisterCommands(object owner, string specific)
        {
            lock (_commands)
            {
                foreach (MethodInfo method in owner.GetType().GetRuntimeMethods())
                {
                    if(method.IsStatic) continue;
                    Command command = method.GetCustomAttribute<Command>();
                    if (command == null) continue;
                    Bridge.Logger.Debug("A new command was found: {name}; Try to register it...", command.Name);
                    if (Bridge.IsCommandOccupied(command.Name))
                    {
                        if (specific == "native")
                        {
                            return;
                        }

                        string newName = specific + ":" + command.Name;
                        Bridge.Logger.Warn("Occupied console command name found, changed it to plugin-specific: {OLD} => {NEW}", command.Name, newName);
                        command.SetCommandName(newName);
                    }
                    
                    Bridge.OccupyCommand(command.Name);
                    Onset.RegisterCommand(_server?.Owner.Plugin.Meta.Id ?? "native", command.Name);
                    foreach (string alias in command.Aliases)
                    {
                        Onset.RegisterCommandAlias(_server?.Owner.Plugin.Meta.Id ?? "native", command.Name, alias);
                    }
                    
                    command.SetHandler(owner, method);
                    _commands.Add(command);
                    CommandInfo.RegisterCommand(command);
                    Bridge.Logger.Debug("A new command was registered: {CMD}!", command.Name);
                }
            }
        }

        internal void RegisterCommands<T>(string specific)
        {
            lock (_commands)
            {
                foreach (MethodInfo method in typeof(T).GetRuntimeMethods())
                {
                    if(!method.IsStatic) continue;
                    Command command = method.GetCustomAttribute<Command>();
                    if (command == null) continue;
                    Bridge.Logger.Debug("A new command was found: {name}; Try to register it...", command.Name);
                    if (Bridge.IsCommandOccupied(command.Name))
                    {
                        if (specific == "native")
                        {
                            return;
                        }
                        
                        string newName = specific + ":" + command.Name;
                        Bridge.Logger.Warn("Occupied console command name found, changed it to plugin-specific: {OLD} => {NEW}", command.Name, newName);
                        command.SetCommandName(newName);
                    }
                    
                    Bridge.OccupyCommand(command.Name);
                    Onset.RegisterCommand(_server?.Owner.Plugin.Meta.Id ?? "native", command.Name);
                    foreach (string alias in command.Aliases)
                    {
                        Onset.RegisterCommandAlias(_server?.Owner.Plugin.Meta.Id ?? "native", command.Name, alias);
                    }
                    
                    command.SetHandler(null, method);
                    _commands.Add(command);
                    CommandInfo.RegisterCommand(command);
                    Bridge.Logger.Debug("A new command was registered: {CMD}!", command.Name);
                }
            }
        }

        /// <summary>
        /// Tries to execute a command with the belonging data derived from the given line of text.<br/>
        /// <br/>
        /// If the command execution fails, a custom event called CommandFailure event is getting called.<br/>
        /// The arguments are: <see cref="Player"/> executor, <see cref="CommandFailure"/> failure, <see cref="string"/> line, <see cref="string"/> commandName
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="line">The data in form of a line of text</param>
        /// <param name="playerId">The player id which executed the command</param>
        internal void ExecuteCommand(string name, string line, int playerId)
        {
            try
            {
                Player player = _server.CreatePlayer(playerId);
                if (player == null)
                {
                    _server.Owner.Plugin.Logger.Fatal("No player was found at the command execution! Missing entity id {ID}!", playerId);
                    return;
                }

                Command command = GetCommand(name);
                if (command == null)
                {
                    _server.CallEventUnsafely("CommandFailure", player, CommandFailure.NoCommand, line, name);
                    return;
                }

                if (!string.IsNullOrEmpty(command.Permission) && !player.HasPermission(command.Permission))
                {
                    _server.CallEventUnsafely("CommandFailure", player, CommandFailure.NoPermissions, line, name);
                    return;
                }
                
                List<string> strArgs = new List<string>();
                string currentStr = null;
                string openingChar = "";

                #region Greedy String Formatting
                
                string[] parts = line.Trim().Split(' ');
                foreach (string str in parts)
                {
                    if(string.IsNullOrEmpty(str?.Trim())) continue;
                    if (currentStr == null)
                    {
                        if (str.StartsWith("\""))
                        {
                            if (str.EndsWith("\""))
                            {
                                strArgs.Add(str.Replace("\"", ""));
                            }
                            else
                            {
                                currentStr = str.Substring(1);
                                openingChar = "\"";
                            }
                        }
                        else if (str.StartsWith("'"))
                        {
                            if (str.EndsWith("'"))
                            {
                                strArgs.Add(str.Replace("'", ""));
                            }
                            else
                            {
                                currentStr = str;
                                openingChar = "'";
                            }
                        }
                        else
                        {
                            strArgs.Add(str);
                        }
                    }
                    else
                    {
                        if (str.EndsWith(openingChar))
                        {
                            currentStr += " " + str.Substring(0, str.Length - 1);
                            strArgs.Add(currentStr);
                            currentStr = null;
                        }
                        else
                        {
                            currentStr += " " + str;
                        }
                    }
                }

                #endregion

                ParameterInfo[] parameters = command.GetParameters();
                if (_server.CallEvent(EventType.PlayerPreCommand, player, name, strArgs.ToArray()))
                {
                    int requiredParams = parameters.Count(info => !info.IsOptional) - 1;
                    if (requiredParams > strArgs.Count)
                    {
                        _server.CallEventUnsafely("CommandFailure", player, CommandFailure.TooFewArgs, line, name);
                        return;
                    }

                    object[] args = Bridge.Convert(_server, strArgs, parameters, player, true);
                    if(args == null) return;
                    command.FireEvent(args);
                }
            }
            catch (Exception ex)
            {
                _server.Owner.Plugin.Logger.Error(ex,
                    "An error occurred while executing an command with line \"{LINE}\" for player {ID}!", line,
                    playerId);
            }
        }

        private Command GetCommand(string name)
        {
            lock (_commands)
            {
                for (int i = _commands.Count - 1; i >= 0; i--)
                {
                    Command command = _commands[i];
                    if (String.Equals(command.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return command;
                    }
                }

                return null;
            }
        }
    }
}