using System;
using System.Runtime.InteropServices;

namespace Onsharp.Native
{
    /// <summary>
    /// The native value is a wrapper for cross-sided values.
    /// </summary>
    internal readonly struct NativeValue : IDisposable
    {
        internal readonly IntPtr NativePtr;
        private readonly Type _type;

        public NativeValue(IntPtr nativePtr)
        {
            NativePtr = nativePtr;
            _type = Onset.GetNType(nativePtr);
        }

        /// <summary>
        /// Gets the value from the native value.
        /// </summary>
        /// <returns>The converted value</returns>
        public object GetValue()
        {
            switch (_type)
            {
                case Type.String:
                    return Marshal.PtrToStringUTF8(Onset.GetNString(NativePtr));
                case Type.Double:
                    return Onset.GetNDouble(NativePtr);
                case Type.Integer:
                    return Onset.GetNInt(NativePtr);
                case Type.Boolean:
                    return Onset.GetNBoolean(NativePtr);
                default:
                    return null;
            }
        }

        public void Dispose()
        {
            Onset.FreeNValue(NativePtr);
        }

        /// <summary>
        /// Defines the type of the value contained by the native value.
        /// </summary>
        public enum Type : byte
        {
            Null,
            String,
            Double,
            Integer,
            Boolean,
            Table
        }
    }
}