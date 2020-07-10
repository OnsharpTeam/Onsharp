using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Onsharp.Native
{
    /// <summary>
    /// This is a collection of all native methods which will call them in the c++ runtime.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class Onset
    {
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateObject(int model, double x, double y, double z, double rx, double ry,
            double rz, double sx, double sy, double sz);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetAllPackages();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPackageStarted([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopPackage([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartPackage([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerName(int player, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerName(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SendPlayerChatMessage(int player, [MarshalAs(UnmanagedType.LPStr)] string message);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsEntityValid(int id, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr[] GetKeysFromTable(IntPtr table);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void AddValueToTable(IntPtr table, IntPtr key, IntPtr val);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RemoveTableKey(IntPtr table, IntPtr key);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ContainsTableKey(IntPtr table, IntPtr key);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetValueFromTable(IntPtr table, IntPtr key);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetLengthOfTable(IntPtr table);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr[] InvokePackage([MarshalAs(UnmanagedType.LPStr)] string importId, [MarshalAs(UnmanagedType.LPStr)] string funcName, IntPtr[] nVals, int len);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ImportPackage([MarshalAs(UnmanagedType.LPStr)] string packageName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetEntityPosition(int id, [MarshalAs(UnmanagedType.LPStr)] string name, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetEntityPosition(int id, [MarshalAs(UnmanagedType.LPStr)] string name, ref double x, ref double y, ref double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ShutdownServer();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterRemoteEvent([MarshalAs(UnmanagedType.LPStr)] string pluginId, [MarshalAs(UnmanagedType.LPStr)] string eventName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterCommand([MarshalAs(UnmanagedType.LPStr)] string pluginId, [MarshalAs(UnmanagedType.LPStr)] string commandName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void CallRemote(int player, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr[] nVals, int len);
        
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

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_t", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValueTable();

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
        internal static extern void ForceRuntimeRestart(bool complete);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ReleaseIntArray(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetEntities([MarshalAs(UnmanagedType.LPStr)] string name, ref int len);

        

    }
}