using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Onsharp.Commands;

namespace Onsharp.Native
{
    /// <summary>
    /// The console manager manages all console commands as well as the input to the console.
    /// </summary>
    internal class ConsoleManager
    {
        private readonly List<ConsoleCommand> _commands;
        private readonly Thread _inputThread;
        private bool _isRunning = true;
        
        internal ConsoleManager()
        {
            _commands = new List<ConsoleCommand>();
            _inputThread = new Thread(OnInput){IsBackground = true, Name = "ConsoleInput"};
            Console.CursorVisible = false;
        }

        private void OnInput()
        {
            Thread.Sleep(Bridge.Config.ConsoleInputTimeout);
            Console.CursorVisible = true;
            while (_isRunning)
            {
                Console.Write("> ");   
                string line = Console.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    string[] parts = line.Split(' ');
                    string name = parts[0];
                    string[] args = new string[parts.Length - 1];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        args[i - 1] = parts[i];
                    }
                    
                    ExecuteCommand(line, name, args);
                }
            }
        }

        private void ExecuteCommand(string line, string name, string[] parts)
        {
            try
            {
                ConsoleCommand command = GetCommand(name);
                if (command == null)
                {
                    Bridge.Logger.Fatal("No such a command found! Consider \"help\" to see all existing commands.");
                    return;
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
                    return;
                }

                object[] args = Bridge.ConvertBasic(strArgs, parameters, true);
                if(args == null) return;
                command.FireEvent(args);
            }
            catch (Exception ex)
            {
                Bridge.Logger.Error(ex,
                    "An error occurred while executing an command with line \"{LINE}\"!", line);
            }
        }

        private ConsoleCommand GetCommand(string name)
        {
            lock (_commands)
            {
                for (int i = _commands.Count - 1; i >= 0; i--)
                {
                    ConsoleCommand command = _commands[i];
                    if (string.Equals(command.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return command;
                    }
                }

                return null;
            }
        }

        internal void PrintCommands()
        {
            lock (_commands)
            {
                Bridge.Logger.Info("All Console Commands:");
                foreach (ConsoleCommand command in _commands)
                {
                    Bridge.Logger.Info(" > " + command.Name +
                                       (string.IsNullOrEmpty(command.Usage) ? "" : " " + command.Usage) + " - " +
                                       command.Description);
                }
            }
        }
        
        internal void Stop()
        {
            _isRunning = false;
            _inputThread.Interrupt();
        }

        internal void Start()
        {
            _inputThread.Start();
        }
        
        internal void Register(object owner)
        {
            lock (_commands)
            {
                foreach (MethodInfo method in owner.GetType().GetRuntimeMethods())
                {
                    if(method.IsStatic) continue;
                    ConsoleCommand command = method.GetCustomAttribute<ConsoleCommand>();
                    if (command == null) continue;
                    command.SetHandler(owner, method);
                    _commands.Add(command);
                }
            }
        }

        internal void Register<T>()
        {
            lock (_commands)
            {
                foreach (MethodInfo method in typeof(T).GetRuntimeMethods())
                {
                    if(!method.IsStatic) continue;
                    ConsoleCommand command = method.GetCustomAttribute<ConsoleCommand>();
                    if (command == null) continue;
                    command.SetHandler(null, method);
                    _commands.Add(command);
                }
            }
        }
    }
}