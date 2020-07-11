using System.Drawing;
using Onsharp.Native;

namespace Onsharp
{
    /// <summary>
    /// An utility class for color converting.
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Converts a RGBA color to a hex string.
        /// </summary>
        /// <param name="red">The red color value</param>
        /// <param name="green">The green color value</param>
        /// <param name="blue">The blue color value</param>
        /// <param name="alpha">The alpha channel</param>
        /// <returns>The hex string</returns>
        public static string RgbaToHex(int red, int green, int blue, int alpha)
        {
            return Bridge.PtrToString(Onset.GetColorHex(red, green, blue, alpha, true));
        }

        /// <summary>
        /// Converts a RGB color to a hex string.
        /// </summary>
        /// <param name="red">The red color value</param>
        /// <param name="green">The green color value</param>
        /// <param name="blue">The blue color value</param>
        /// <returns>The hex string</returns>
        public static string RgbToHex(int red, int green, int blue)
        {
            return Bridge.PtrToString(Onset.GetColorHex(red, green, blue, 0, false));
        }

        /// <summary>
        /// Converts a color object to HEX color.
        /// </summary>
        /// <param name="color">The color object</param>
        /// <returns>The HEX color</returns>
        public static string ColorToHex(Color color)
        {
            return RgbaToHex(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Converts a HEX color to a color object.
        /// </summary>
        /// <param name="hex">The HEX string</param>
        /// <returns>The color object</returns>
        public static Color HexToColor(string hex)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            int alpha = 0;
            Onset.GetColorValuesFromHex(hex, ref red, ref green, ref blue, ref alpha);
            return Color.FromArgb(alpha, red, green, blue);
        }
    }
}