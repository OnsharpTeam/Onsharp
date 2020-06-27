using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Onsharp.Native;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SLogger = Serilog.Core.Logger;

namespace Onsharp.IO
{
    internal class Logger : ILogger
    {
        private readonly SLogger _internalLogger;
        private readonly string _prefix;
        private readonly bool _isDebug;
        private readonly string _path;

        internal Logger(string prefix, bool isDebug, string pathName = null)
        {
            _prefix = prefix;
            _path = Path.Combine(Bridge.LogPath, pathName ?? prefix);
            Directory.CreateDirectory(_path);
            _isDebug = isDebug;
            _internalLogger = new LoggerConfiguration().WriteTo.Console().WriteTo.Sink(new EventLogSink(this)).CreateLogger();
        }

        private string Format(string message)
        {
            return $"[{_prefix}] {message}";
        }
        
        public void Info(string message, params object[] args)
        {
            _internalLogger.Information(Format(message), args);
        }

        public void Warn(string message, params object[] args)
        {
            _internalLogger.Warning(Format(message), args);
        }

        public void Verbose(string message, params object[] args)
        {
            _internalLogger.Verbose(Format(message), args);
        }

        public void Debug(string message, params object[] args)
        {
            if(!_isDebug) return;
            _internalLogger.Debug(Format(message), args);
        }

        public void Fatal(string message, params object[] args)
        {
            _internalLogger.Fatal(Format(message), args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            _internalLogger.Fatal(Format(message) + "\n" + exception, args);
        }
        
        private class EventLogSink : ILogEventSink
        {
            private readonly ReaderWriterLock _locker = new ReaderWriterLock();
            private readonly Logger _parent;

            internal EventLogSink(Logger parent)
            {
                _parent = parent;
            }

            public void Emit(LogEvent logEvent)
            {
                string line = $"[{DateTime.Now:hh:mm:ss} {LevelToSeverity(logEvent)}] " + logEvent.RenderMessage();
                try
                {
                    _locker.AcquireWriterLock(int.MaxValue);
                    File.AppendAllLines(Path.Combine(_parent._path, DateTime.Today.ToString("dd_MM_yyyy") + ".txt"), new[] { line });
                }
                finally
                {
                    _locker.ReleaseWriterLock();
                }
            }

            private string LevelToSeverity(LogEvent logEvent)
            {
                switch (logEvent.Level)
                {
                    case LogEventLevel.Debug:
                        return "DBG";
                    case LogEventLevel.Error:
                        return "ERR";
                    case LogEventLevel.Fatal:
                        return "FTL";
                    case LogEventLevel.Verbose:
                        return "VRB";
                    case LogEventLevel.Warning:
                        return "WRN";
                    default:
                        return "INF";
                }
            }
        }
    }
}