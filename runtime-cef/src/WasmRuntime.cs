using System;
using System.Reflection;

namespace Onsharp.Client
{
    internal class WasmRuntime 
    {
        private Assembly _assembly; 

        public void Load(string path) 
        {
            Console.WriteLine("This path is the DLL: " + path);
        }
    }
}