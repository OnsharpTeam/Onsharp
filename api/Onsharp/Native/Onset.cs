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
        internal static extern void CallRemote(long player, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr[] nVals, int len);
        
        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_s", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue([MarshalAs(UnmanagedType.LPStr)] string val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_i", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue(int val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_d", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue(double val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_b", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue(bool val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_n", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue();

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FreeNValue(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeValue.Type GetNType(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetNDouble(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetNInt(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetNBoolean(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetNString(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FreeVPtr(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetVDouble(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern float GetVFloat(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetVBool(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetVString(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerName(long player, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerName(long player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SendPlayerChatMessage(long player, [MarshalAs(UnmanagedType.LPStr)] string message);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ForceRuntimeRestart(bool complete);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsEntityValid(long id, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ReleaseIntArray(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetEntities([MarshalAs(UnmanagedType.LPStr)] string name, ref int len);

        

    }
}