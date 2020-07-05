using System;
using System.Text.RegularExpressions;
using Onsharp.Native;

namespace Onsharp.Interop
{
    /// <summary>
    /// The lua package defines a imported package in the current runtime of Onsharp.
    /// Lua packages hold meta information about the entry point to interop functions of these imported packages.
    /// The class serves as pipeline to access the exported functions.
    /// </summary>
    public class LuaPackage
    {
        private readonly string _importId;
        
        /// <summary>
        /// The default constructor which passes the needed meta information to generate the import id.
        /// </summary>
        /// <param name="pluginId">The plugin id which is the owner of the imported package</param>
        /// <param name="packageName">The name of the package which should be imported</param>
        internal LuaPackage(string pluginId, string packageName)
        {
            _importId = Regex.Replace(pluginId, "[^a-zA-Z]", "") + "_" + packageName;
        }

        /// <summary>
        /// Invokes the wanted function by the given name and the given arguments.
        /// No returning value is parsed and returned.
        /// </summary>
        /// <param name="funcName">The name of the function to be executed</param>
        /// <param name="args">The parameters which will be passed to the function</param>
        public void Invoke(string funcName, params object[] args)
        {
            InvokeMultiple(funcName, args);
        }

        /// <summary>
        /// Invokes the wanted function by the given name and the given arguments.
        /// The return value of this is the first one casted to the wanted type.
        /// </summary>
        /// <param name="funcName">The name of the function to be executed</param>
        /// <param name="args">The parameters which will be passed to the function</param>
        /// <returns>The casted return value or default if none is returned</returns>
        public T Invoke<T>(string funcName, params object[] args)
        {
            object[] returnValues = InvokeMultiple(funcName, args);
            if (returnValues.Length <= 0) return default;
            return (T) returnValues[0];
        }
        
        /// <summary>
        /// Invokes the wanted function by the given name and the given arguments.
        /// The invoke will return an array of objects which got returned by this function.
        /// If nothing is returned, it is simply empty.
        /// </summary>
        /// <param name="funcName">The name of the function to be executed</param>
        /// <param name="args">The parameters which will be passed to the function</param>
        /// <returns>An array of object as return values</returns>
        public object[] InvokeMultiple(string funcName, params object[] args)
        {
            IntPtr[] nVals = new IntPtr[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                nVals[i] = Bridge.CreateNValue(args[i]).NativePtr;
            }

            IntPtr[] rVals = Onset.InvokePackage(_importId, funcName, nVals, args.Length);
            object[] rArgs = new object[rVals.Length];
            for (int i = 0; i < rVals.Length; i++)
            {
                rArgs[i] = new NativeValue(rVals[i]).GetValue();
            }
            
            return rArgs;
        }
    }
}