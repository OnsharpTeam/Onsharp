using System;

namespace Onsharp.Exceptions
{
    /// <summary>
    /// This exception gets thrown by the <see cref="Plugins.PluginManager"/>.
    /// </summary>
    public class PluginException : Exception
    {
        internal PluginException(string message) : base(message)
        {
        }
    }
}