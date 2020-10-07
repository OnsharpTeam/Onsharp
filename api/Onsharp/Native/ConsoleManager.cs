using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Onsharp.Commands;
using Onsharp.Utils;

namespace Onsharp.Native
{
    /// <summary>
    /// The console manager manages all console commands as well as the input to the console.
    /// </summary>
    internal class ConsoleManager
    {
        private const int CommandsPerPage = 5;
        private readonly List<ConsoleCommand> _commands;
        
        internal ConsoleManager()
        {
            _commands = new List<ConsoleCommand>();
        }

        internal void Reset()
        {
            lock (_commands)
            {
                _commands.Clear();
            }
            
            Register(Bridge.Runtime, "native", true);
        }
        
        internal bool PollInput(string input, bool native = false)
        {
            if (input.StartsWith("/"))
            {
                input = input.Substring(1);
                if (!string.IsNullOrEmpty(input))
                {
                    string[] parts = input.Split(' ');
                    string name = parts[0];
                    string[] args = new string[parts.Length - 1];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        args[i - 1] = parts[i];
                    }
                    
                    return ExecuteCommand(input, name, args, native);
                }
            }

            return false;
        }

        private bool ExecuteCommand(string line, string name, string[] parts, bool native = false)
        {
            try
            {
                ConsoleCommand command = GetCommand(name, native);
                if (command == null)
                {
                    if(!native)
                        Bridge.Logger.Fatal("No such a command found! Consider \"help\" to see all existing commands.");
                    return false;
                }
                
                List<string> strArgs = new List<string>();
                string currentStr = null;
                string openingChar = "";

                #region Greedy String Formatting

                foreach (string str in parts)
                {
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
                                currentStr = str;
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
                int requiredParams = parameters.Count(info => !info.IsOptional) - 1;
                if (requiredParams > strArgs.Count)
                {
                    Bridge.Logger.Fatal("Not enough arguments!");
                    return true;
                }

                object[] args = Bridge.ConvertBasic(strArgs, parameters, true);
                if(args == null) return true;
                command.FireEvent(args);
            }
            catch (Exception ex)
            {
                Bridge.Logger.Error(ex,
                    "An error occurred while executing an command with line \"{LINE}\"!", line);
            }
            return true;
        }

        private ConsoleCommand GetCommand(string name, bool native)
        {
            lock (_commands)
            {
                for (int i = _commands.Count - 1; i >= 0; i--)
                {
                    ConsoleCommand command = _commands[i];
                    if (command.IsNative != native) continue;
                    if (string.Equals(command.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return command;
                    }

                    if (command.Aliases.ContainsIgnoreCase(name))
                    {
                        return command;
                    }
                }

                return null;
            }
        }

        internal void PrintCommands(int page)
        {
            lock (_commands)
            {
                int pages = CountPages();
                if(page > pages) return;
                string header = page + "/" + pages;
                Bridge.Logger.Info("<============[ Help ({PAGES}) ]============>", header);
                int start = CommandsPerPage * (page - 1);
                int end = start + CommandsPerPage - 1;

                for (int i = start; i <= end; i++)
                {
                    if(_commands.Count <= i) break;
                    ConsoleCommand command = _commands[i];
                    PrintCommand(command);
                }
                
                Bridge.Logger.Info("<============[ Help ({PAGES}) ]============>", header);
            }
        }

        private void PrintCommand(ConsoleCommand command)
        {
            Bridge.Logger.Info("{COMMAND}" + command.Usage.Text + " : " + command.Description, command.CommandText);
            foreach (Usage.Parameter parameter in command.Usage.Parameters)
            {
                Bridge.Logger.Info("    {NAME} (" + parameter.Type + "): " + parameter.Description, parameter.Name);
            }
        }
        
        internal void Register(object owner, string specific, bool native = false)
        {
            lock (_commands)
            {
                foreach (MethodInfo method in owner.GetType().GetRuntimeMethods())
                {
                    if(method.IsStatic) continue;
                    ConsoleCommand command = method.GetCustomAttribute<ConsoleCommand>();
                    if (command == null) continue;
                    Bridge.Logger.Debug("A new console command was found: {name}; Try to register it...", command.Name);
                    if (Bridge.IsConsoleCommandOccupied(command.Name))
                    {
                        if (specific == "native")
                        {
                            return;
                        }
                        
                        if (string.IsNullOrEmpty(specific))
                        {
                            Bridge.Logger.Fatal(
                                "Console command {NAME} could not be registered because this name is occupied and no specifier was passed!",
                                command.Name);
                            continue;
                        }

                        string newName = specific + ":" + command.Name;
                        Bridge.Logger.Warn("Occupied console command name found, changed it to plugin-specific: {OLD} => {NEW}", command.Name, newName);
                        command.SetCommandName(newName);
                    }
                    
                    Bridge.OccupyConsoleCommand(command.Name);
                    command.SetHandler(owner, method);
                    command.IsNative = native;
                    _commands.Add(command);
                    Bridge.Logger.Debug("A new console command was registered: {CMD}!", command.Name);
                }
            }
        }

        internal void Register<T>(string specific)
        {
            lock (_commands)
            {
                foreach (MethodInfo method in typeof(T).GetRuntimeMethods())
                {
                    if(!method.IsStatic) continue;
                    ConsoleCommand command = method.GetCustomAttribute<ConsoleCommand>();
                    if (command == null) continue;
                    Bridge.Logger.Debug("A new console command was found: {name}; Try to register it...", command.Name);
                    if (Bridge.IsConsoleCommandOccupied(command.Name))
                    {
                        if (specific == "native")
                        {
                            return;
                        }
                        
                        if (string.IsNullOrEmpty(specific))
                        {
                            Bridge.Logger.Fatal(
                                "Console command {NAME} could not be registered because this name is occupied and no specifier was passed!",
                                command.Name);
                            continue;
                        }
                        
                        string newName = specific + ":" + command.Name;
                        Bridge.Logger.Warn("Occupied console command name found, changed it to plugin-specific: {OLD} => {NEW}", command.Name, newName);
                        command.SetCommandName(newName);
                    }
                    
                    Bridge.OccupyConsoleCommand(command.Name);
                    command.SetHandler(null, method);
                    _commands.Add(command);
                    Bridge.Logger.Debug("A new console command was registered: {CMD}!", command.Name);
                }
            }
        }

        private int CountPages()
        {
            lock (_commands)
            {
                return (int) Math.Ceiling(_commands.Count / (float) CommandsPerPage);
            }
        }
    }
}