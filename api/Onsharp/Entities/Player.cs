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
            get => Bridge.PtrToString(Onset.GetPlayerName(Id));
            set => Onset.SetPlayerName(Id, value);
        }
        
        public Player(long id) : base(id, "Player")
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
    }
}