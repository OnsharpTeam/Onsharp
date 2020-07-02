using Onsharp.Commands;
using Onsharp.Entities;
using Onsharp.Plugins;

namespace TestPlugin
{
    [PluginMeta("test-plugin", "TestPlugin", "1.0", "OnsharpTeam", IsDebug = true)]
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
            Logger.Fatal("Stopping!");
        }

        [Command("hello")]
        public void OnHelloCommand(Player player)
        {
            player.SendMessage("Hallo Welt!");
        }
    }
}