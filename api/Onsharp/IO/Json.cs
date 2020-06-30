using System;
using Newtonsoft.Json;

namespace Onsharp.IO
{
    /// <summary>
    /// A util class allowing JSON serialization.
    /// </summary>
    public static class Json
    {
        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings();
        private static readonly JsonSerializerSettings TypedSettings = new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.All };
        
        /// <summary>
        /// Converts the given object to a JSON string by the given flags.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="flags">The option flags</param>
        /// <returns>The JSON string</returns>
        public static string ToJson(object obj, Flag flags = Flag.None)
        {
            return JsonConvert.SerializeObject(obj, flags.HasFlag(Flag.Pretty) ? Formatting.Indented : Formatting.None,
                flags.HasFlag(Flag.Typed) ? TypedSettings : DefaultSettings);
        }

        /// <summary>
        /// Converts the given JSON string to an object of the desired type by the given flags.
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <param name="flags">The option flags. If the JSON string is typed, the typed flag is required. The pretty flag is not necessary</param>
        /// <typeparam name="T">The desired type for the object</typeparam>
        /// <returns>The deserialized object</returns>
        public static T FromJson<T>(string json, Flag flags = Flag.None)
        {
            return JsonConvert.DeserializeObject<T>(json,
                flags.HasFlag(Flag.Typed) ? TypedSettings : DefaultSettings);
        }

        /// <summary>
        /// Defines specific options for the JSON serialization.
        /// </summary>
        [Flags]
        public enum Flag
        {
            /// <summary>
            /// The default flag
            /// </summary>
            None = 0,
            /// <summary>
            /// When pretty is enabled, the JSON string will be prettified.
            /// </summary>
            Pretty = 1, 
            /// <summary>
            /// When typed is enabled, the object classes will be entered into the JSON string.
            /// </summary>
            Typed = 2
        }
    }
}