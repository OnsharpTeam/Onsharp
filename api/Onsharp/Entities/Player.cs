using System;
using Onsharp.Events;
using Onsharp.Native;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Player : Entity
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name
        {
            get => Bridge.PtrToString(Onset.GetPlayerName(Id));
            set => Onset.SetPlayerName(Id, value);
        }

        /// <summary>
        /// The steam of the player. Only available after the <see cref="EventType.PlayerSteamAuth"/> was called.
        ///
        /// NOT IMPLEMENTED YET!
        /// </summary>
        public ulong SteamID { get; }
        
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
        
        /// <summary>
        /// Attaches the given object to the entity.
        /// </summary>
        /// <param name="obj">The object which gets attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Object obj, Vector pos, Vector rot, string socketName = "")
        {
            obj.Attach(this, pos, rot, socketName);
        }
        
        /// <summary>
        /// Attaches the given object to the entity.
        /// </summary>
        /// <param name="text3d">The object which gets attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Text3D text3d, Vector pos, Vector rot, string socketName = "")
        {
            text3d.Attach(this, pos, rot, socketName);
        }

        /// <summary>
        /// Sets the visibility of the given pickup for the player.
        /// </summary>
        /// <param name="pickup">The pickup for which the visibility should be changed</param>
        /// <param name="visible">True, if the pickup should be visible</param>
        public void SetPickupVisibility(Pickup pickup, bool visible)
        {
            Onset.SetPickupVisibility(pickup.Id, Id, visible);
        }

        /// <summary>
        /// Sets the visibility of the given 3D text for the player.
        /// </summary>
        /// <param name="text3d">The 3D text for which the visibility should be changed</param>
        /// <param name="visible">True, if the pickup should be visible</param>
        public void SetText3DVisibility(Text3D text3d, bool visible)
        {
            Onset.SetText3DVisibility(text3d.Id, Id, visible);
        }
    }
}