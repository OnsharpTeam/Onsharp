using System;
using System.Runtime.InteropServices;

namespace Onsharp.Native
{
    /// <summary>
    /// The native value is a wrapper for cross-sided values.
    /// </summary>
    internal readonly struct NativeValue : IDisposable
    {
        private readonly IntPtr _nativePtr;
        private readonly Type _type;

        public NativeValue(IntPtr nativePtr)
        {
            _nativePtr = nativePtr;
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
                    return Marshal.PtrToStringUTF8(Onset.GetNString(_nativePtr));
                case Type.Double:
                    return Onset.GetNDouble(_nativePtr);
                case Type.Integer:
                    return Onset.GetNInt(_nativePtr);
                case Type.Boolean:
                    return Onset.GetNBoolean(_nativePtr);
                default:
                    return null;
            }
        }

        public void Dispose()
        {
            Onset.FreeNValue(_nativePtr);
        }

        public enum Type : byte
        {
            Null,
            String,
            Double,
            Integer,
            Boolean
        }
    }
}