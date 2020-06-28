using System.Runtime.InteropServices;
using System.Security;

namespace Onsharp.Native
{
    /// <summary>
    /// This is a collection of all native methods which will call them in the c++ runtime.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class Onset
    {
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsEntityValid(long id, [MarshalAs(UnmanagedType.LPStr)] string name);
    }
}