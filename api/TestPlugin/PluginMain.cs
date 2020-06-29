using Onsharp.Plugins;

namespace TestPlugin
{
    [PluginMeta("test-plugin", "TestPlugin", "1.0", "OnsharpTeam")]
    public class PluginMain : Plugin
    {
        public override void OnStart()
        {
            var config = Data.Config<MyConfig>();
            Logger.SetDebug(config.IsDebug);
            Logger.Debug("Debug mode is {STATE}!", "enabled");
        }

        public override void OnStop()
        {
            
        }
    }
}