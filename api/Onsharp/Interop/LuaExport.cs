using System;
using System.Reflection;
using Onsharp.Native;

namespace Onsharp.Interop
{
    /// <summary>
    /// A method marked with the lua export attribute will be marked importable for LUA scripts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LuaExport : Attribute
    {
        /*
         C#:
         
         [LuaExport("add")]
         public int Add(int val1, int val2);
         
         LUA:
         local add = ImportOnsharpFunc("test-plugin", "add");
         
         local sum = add(1, 1);
         */
        
        /// <summary>
        /// The name of the function how it will be imported into LUA.
        /// The name will define the name which has to be requested.
        /// </summary>
        public string Name { get; }
        
        private MethodInfo _handler;
        private object _owner;

        public LuaExport(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Sets the handling data for this exportable.
        /// </summary>
        /// <param name="owner">The owner of the handler, if static, the owner is null</param>
        /// <param name="handler">The handling method as handler</param>
        internal void SetHandler(object owner, MethodInfo handler)
        {
            _handler = handler;
            _owner = owner;
        }

        /// <summary>
        /// Executes the handler at returns the defined native value which is the return value.
        /// </summary>
        /// <param name="args">The arguments which will be passed to the handler</param>
        /// <returns>The return value</returns>
        internal object Execute(object[] args)
        {
            return _handler.Invoke(_owner, args);
        }
    }
}