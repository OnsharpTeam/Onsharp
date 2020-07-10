using System;
using Onsharp.Native;

namespace Onsharp.Interop
{
    /// <summary>
    /// The lua table represents a lua table on the c++ side. It wraps it functionality up and allows using of it.
    /// </summary>
    public class LuaTable
    {
        /// <summary>
        /// The underlying lua table on the c++ side.
        /// </summary>
        internal NativeValue NVal { get; }

        /// <summary>
        /// The count of the elements (keys) which are in the table currently.
        /// </summary>
        public int Length => Onset.GetLengthOfTable(NVal.NativePtr);

        /// <summary>
        /// An array containing all the keys of this table.
        /// </summary>
        public object[] Keys
        {
            get
            {
                IntPtr[] ptrs = Onset.GetKeysFromTable(NVal.NativePtr);
                object[] keys = new object[ptrs.Length];
                for(int i = 0; i < ptrs.Length; i++)
                {
                    keys[i] = new NativeValue(ptrs[i]).NativePtr;
                }

                return keys;
            }
        }

        /// <summary>
        /// Returns the associated value by the given key.
        /// No existence check is made.
        /// </summary>
        /// <param name="key">The key of the wanted value</param>
        public object this[object key] => GetInternal(key);

        public LuaTable()
        {
            NVal = new NativeValue(Onset.CreateNValueTable());
        }

        internal LuaTable(IntPtr ptr)
        {
            NVal = new NativeValue(ptr);
        }

        /// <summary>
        /// Checks if the given key is present in this table.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if the key is present</returns>
        public bool ContainsKey(object key)
        {
            NativeValue nKey = Bridge.CreateNValue(key);
            return Onset.ContainsTableKey(NVal.NativePtr, nKey.NativePtr);
        }

        /// <summary>
        /// Adds the given key and value to the table.
        /// </summary>
        /// <param name="key">The key to be added</param>
        /// <param name="val">The value to be added</param>
        public void Add(object key, object val)
        {
            NativeValue nKey = Bridge.CreateNValue(key);
            NativeValue nVal = Bridge.CreateNValue(val);
            Onset.AddValueToTable(NVal.NativePtr, nKey.NativePtr, nVal.NativePtr);
        }

        /// <summary>
        /// Removes the key and the associated value in the table.
        /// </summary>
        /// <param name="key">The key which should be removed</param>
        public void Remove(object key)
        {
            NativeValue nKey = Bridge.CreateNValue(key);
            Onset.RemoveTableKey(NVal.NativePtr, nKey.NativePtr);
        }

        /// <summary>
        /// Gets a value by the given key.
        /// </summary>
        /// <param name="key">The key of the wanted value</param>
        /// <returns>The value of the given key or null if not existing</returns>
        public object Get(object key)
        {
            if (!ContainsKey(key)) return null;
            return GetInternal(key);
        }

        private object GetInternal(object key)
        {
            if (key is int idx)
            {
                key = idx + 1;
            }
            
            NativeValue nKey = Bridge.CreateNValue(key);
            return new NativeValue(Onset.GetValueFromTable(NVal.NativePtr, nKey.NativePtr)).GetValue();
        }
    }
}