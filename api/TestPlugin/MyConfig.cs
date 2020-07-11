using Onsharp.IO;

namespace TestPlugin
{
    [Config("myconfig", "")]
    public class MyConfig
    {
        public bool IsDebug { get; set; } = false;
    }
}