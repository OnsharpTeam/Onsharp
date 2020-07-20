namespace Onsharp.Steam
{
    /// <summary>
    /// SteamID Regex constants
    /// </summary>
    internal class SteamIDRegex
    {
        /// <summary>
        /// SteamID2 Regex
        /// </summary>
        public const string Steam2Regex = "^STEAM_0:[0-1]:([0-9]{1,10})$";
        /// <summary>
        /// SteamID32 Regex
        /// </summary>
        public const string Steam32Regex = "^U:1:([0-9]{1,10})$";
        /// <summary>
        /// SteamID64 Regex
        /// </summary>
        public const string Steam64Regex = "^7656119([0-9]{10})$";
    }
}