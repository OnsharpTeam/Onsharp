using System;
using System.IO;
using System.Reflection;
using WebAssembly;

namespace Onsharp.Client 
{
    public class Program 
    {
        private static WasmRuntime _runtime;

        static void Main()
        {
            _runtime = new WasmRuntime();
        }

        static void StartRuntime()
        {
            Console.WriteLine("Starting WASM runtime...");
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Onsharp.Client.dll");
            _runtime.Load(path);
        }
    }
}