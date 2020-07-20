using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Onsharp.Steam
{
    /// <summary>
    /// SteamId converting class.
    /// </summary>
    internal static class SteamIDConvert
    {
        /// <summary>
        /// Converts a <see cref="AuthIdType.AuthId_Steam3"/> to a <see cref="AuthIdType.AuthId_Steam2"/> format.
        /// </summary>
        /// <param name="input">String input of AuthId_Steam3</param>
        /// <returns>Returns the SteamID2(STEAM_0:1:000000) string.</returns>
        internal static string Steam32ToSteam2(string input)
        {
            if (!Regex.IsMatch(input, SteamIDRegex.Steam32Regex))
            {
                return string.Empty;
            }
            long steam64 = Steam32ToSteam64(input);
            return Steam64ToSteam2(steam64);
        }

        /// <summary>
        /// Converts a <see cref="AuthIdType.AuthId_Steam2"/> to a <see cref="AuthIdType.AuthId_Steam3"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string Steam2ToSteam32(string input)
        {
            if (!Regex.IsMatch(input, SteamIDRegex.Steam2Regex))
            {
                return string.Empty;
            }
            long steam64 = Steam2ToSteam64(input);
            return Steam64ToSteam32(steam64);
        }

        /// <summary>
        /// Converts our <see cref="AuthIdType.AuthId_Steam3"/> to the <see cref="AuthIdType.AuthId_SteamID64"/> format.
        /// </summary>
        /// <param name="input">AuthId_Steam3</param>
        /// <returns>Returns the SteamID64(76561197960265728) in long type</returns>
        internal static long Steam32ToSteam64(string input)
        {
            long steam32 = Convert.ToInt64(input.Substring(4));
            if (steam32 < 1L || !Regex.IsMatch("U:1:" + steam32.ToString(CultureInfo.InvariantCulture), "^U:1:([0-9]{1,10})$"))
            {
                return 0;
            }
            return steam32 + 76561197960265728L;
        }

        /// <summary>
        /// Converts a <see cref="AuthIdType.AuthId_SteamID64"/> to a <see cref="AuthIdType.AuthId_Steam2"/>
        /// </summary>
        /// <param name="communityId">SteamID64(76561197960265728)</param>
        /// <returns>String.empty if error, else the string SteamID2(STEAM_0:1:000000)</returns>
        internal static string Steam64ToSteam2(long communityId)
        {
            if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString(CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
                return string.Empty;
            communityId -= 76561197960265728L;
            long num = communityId % 2L;
            communityId -= num;
            string input = $"STEAM_0:{num}:{(communityId / 2L)}";
            if (!Regex.IsMatch(input, "^STEAM_0:[0-1]:([0-9]{1,10})$"))
            {
                return string.Empty;
            }
            return input;
        }

        /// <summary>
        /// Converts a <see cref="AuthIdType.AuthId_Steam2"/> to a <see cref="AuthIdType.AuthId_SteamID64"/>
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Returns a <see cref="AuthIdType.AuthId_SteamID64"/></returns>
        internal static long Steam2ToSteam64(string accountId)
        {
            if (!Regex.IsMatch(accountId, "^STEAM_0:[0-1]:([0-9]{1,10})$"))
            {
                return 0;
            }
            return 76561197960265728L + Convert.ToInt64(accountId.Substring(10)) * 2L + Convert.ToInt64(accountId.Substring(8, 1));
        }

        /// <summary>
        /// Converts a <see cref="AuthIdType.AuthId_SteamID64"/> to a <see cref="AuthIdType.AuthId_Steam3"/>
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns>Returns a <see cref="AuthIdType.AuthId_Steam3"/> string</returns>
        internal static string Steam64ToSteam32(long communityId)
        {
            if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString(CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
            {
                return string.Empty;
            }
            return $"U:1:{communityId - 76561197960265728L}";
        }

    }
}