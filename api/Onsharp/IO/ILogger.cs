using System;

namespace Onsharp.IO
{
    /// <summary>
    /// The logger is a tool to log messages to the console and the log file system of Onsharp.
    /// Every plugin has its own logger and can use it via the <see cref="Onsharp.Plugin.IPlugin"/> file.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Prints an info message to the logging system.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Prints a warning message to the logging system.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// Prints a verbose message to the logging system.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
        void Verbose(string message, params object[] args);

        /// <summary>
        /// Prints a debug message to the logging system.
        /// This message is only getting printed, when Onsharp is in Debug mode, or the plugin itself.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Prints a fatal message to the logging system.
        /// </summary>
        /// <param name="message">The message to be printed</param>
        /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Prints an error message to the logging system.
        /// </summary>
        /// <param name="exception">The exception which occurred</param>
        /// <param name="message">The message to be printed</param>
        /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
        void Error(Exception exception, string message, params object[] args);

        /// <summary>
        /// Manually setting the debug mode enabled or disabled.
        /// </summary>
        /// <param name="enabled">True if the debug mode should be enabled</param>
        void SetDebug(bool enabled);
    }
}