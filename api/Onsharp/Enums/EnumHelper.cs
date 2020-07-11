using System;

namespace Onsharp.Enums
{
    /// <summary>
    /// Represents a helper class for the enums in this "Enums" folder.
    /// </summary>
    public static class EnumHelper
    {
        private static readonly Type AnimationType = typeof(Animation);
        private static readonly Type WeaponStatType = typeof(WeaponStat);

        /// <summary>
        /// Returns the name of the given animation, or null.
        /// </summary>
        /// <param name="animation">The animation</param>
        /// <returns>The animation name or null if it fails</returns>
        public static string GetName(this Animation animation)
        {
            return Enum.GetName(AnimationType, animation)?.ToUpper();
        }

        /// <summary>
        /// Returns the name of the given weapon stat, or null.
        /// </summary>
        /// <param name="stat">The weapon stat</param>
        /// <returns>The weapon stat name or null if it fails</returns>
        public static string GetName(this WeaponStat stat)
        {
            return Enum.GetName(WeaponStatType, stat)?.ToUpper();
        }
    }
}