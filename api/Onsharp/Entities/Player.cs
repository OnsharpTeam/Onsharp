using System;
using System.Collections.Generic;
using Onsharp.Native;

namespace Onsharp.Entities
{
    public class Player : Entity
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name
        {
            get => (string) new NativeValue(Onset.GetPlayerName(Id)).GetValue();
            set => Onset.SetPlayerName(Id, value);
        }
        
        public Player(int id) : base(id, "Player")
        {
        }

        /// <summary>
        /// Sends a message to the player's chat.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="args">The arguments which should be passed to the string</param>
        public void SendMessage(string message, params object[] args)
        {
            string formattedString = string.Format(message, args);
            Onset.SendPlayerChatMessage(Id, formattedString);
        }

        /// <summary>
        /// Calls a remote event handler on the client-side of this player.
        /// </summary>
        /// <param name="name">The name of the remote event handler which should be called</param>
        /// <param name="args">The arguments which will passed to the event handler. The maximum length is 10</param>
        public void CallRemote(string name, params object[] args)
        {
            if(args.Length > 10)
                throw new ArgumentException("The maximum length of event handler arguments are 10!");

            IntPtr[] argsArr = new IntPtr[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                argsArr[i] = Bridge.CreateNValue(args[i]).NativePtr;
            }
            
            Onset.CallRemote(Id, name, argsArr, argsArr.Length);
        }
    }
}