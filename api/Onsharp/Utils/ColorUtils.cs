using System.Drawing;
using Onsharp.Native;

namespace Onsharp.Utils
{
    /// <summary>
    /// An utility class for color converting.
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Converts a RGB color to a hex string.
        /// </summary>
        /// <param name="red">The red color value</param>
        /// <param name="green">The green color value</param>
        /// <param name="blue">The blue color value</param>
        /// <returns>The hex int string</returns>
        public static string RgbToHex(int red, int green, int blue)
        {
            int rgb = red;
            rgb = (rgb << 8) + green;
            rgb = (rgb << 8) + blue;
            return rgb.ToString();
        }

        /// <summary>
        /// Converts a color object to HEX color.
        /// </summary>
        /// <param name="color">The color object</param>
        /// <returns>The HEX color</returns>
        public static string ColorToHex(Color color)
        {
            return RgbToHex(color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a HEX color to a color object.
        /// </summary>
        /// <param name="hex">The HEX int string</param>
        /// <returns>The color object</returns>
        public static Color HexToColor(string hex)
        {
            if (!int.TryParse(hex, out int rgb)) return Color.Black;
            int red = (rgb >> 16) & 0xFF;
            int green = (rgb >> 8) & 0xFF;
            int blue = rgb & 0xFF;
            return Color.FromArgb(red, green, blue);
        }
    }
}