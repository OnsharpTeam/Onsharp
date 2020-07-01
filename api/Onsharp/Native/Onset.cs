using System;
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
        internal static extern void SetPlayerName(long player, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerName(long player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SendPlayerChatMessage(long player, [MarshalAs(UnmanagedType.LPStr)] string message);
        
        [DllImport(Bridge.DllName)]
        internal static extern void ForceRuntimeRestart(bool complete);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsEntityValid(long id, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName)]
        internal static extern int ReleaseLongArray(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetEntities([MarshalAs(UnmanagedType.LPStr)] string name, ref int len);

        

    }
}