namespace Onsharp.Steam
{
    /// <summary>
    /// Auth type enumeration, From SourcePawn's Clients.inc
    /// </summary>
    internal enum AuthIdType
    {
        /// <summary>
        /// The game-specific auth string as returned from the engine
        /// </summary>
        AuthId_Engine = 0,
        // The following are only available on games that support Steam authentication.
        /// <summary>
        /// Steam2 rendered format, ex "STEAM_1:1:4153990"
        /// </summary>
        AuthId_Steam2,
        /// <summary>
        /// Steam3 rendered format, ex "U:1:8307981"
        /// </summary>
        AuthId_Steam3,
        /// <summary>
        /// A SteamID64 (uint64) as a String, ex "76561197968573709"
        /// </summary>
        AuthId_SteamID64,
    }
}