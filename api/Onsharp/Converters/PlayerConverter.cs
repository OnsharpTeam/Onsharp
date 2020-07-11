using System;
using System.Reflection;
using Onsharp.Entities;
using Onsharp.Native;

namespace Onsharp.Converters
{
    /// <summary>
    /// The player converter takes all kinds of player 
    /// </summary>
    internal class PlayerConverter : Converter
    {
        public PlayerConverter() : base(null, null)
        {
            
        }

        internal override bool IsHandlerFor(ParameterInfo parameter)
        {
            return typeof(Player).IsAssignableFrom(parameter.ParameterType);
        }

        internal override object Handle(string value, ParameterInfo parameter, Server server)
        {
            if (long.TryParse(value, out long val))
            {
                return server.GetPlayerBy(player => player.SteamID == val);
            }

            return server.GetPlayerBy(player => player.Name == value);
        }
    }
}