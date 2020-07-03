using Onsharp.Commands;
using Onsharp.Entities;
using Onsharp.Events;
using Onsharp.Plugins;
using Onsharp.Updater;

namespace TestPlugin
{
    [AutoUpdater("https://eternitylife.de/test.json")]
    [PluginMeta("test-plugin", "TestPlugin", "1.1", "OnsharpTeam", IsDebug = true)]
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

        [ServerEvent("CommandFailure")]
        public void OnCommandFailure(Player player, CommandFailure failure, string line, string name)
        {
            Logger.Fatal("command failure {FAIL} with {NAME} and {LINE} for {PLAYER}", failure, name, line, player.Name);
        }

        [ServerEvent(EventType.ClientConnectionRequest)]
        public void OnConnectionReq(string ip, int port)
        {
            Logger.Debug("incoming request {IP}:{PORT}", ip, port);
        }

        [Command("hello")]
        public void OnHelloCommand(Player player)
        {
            player.CallRemote("trigger-val", "Hallo ");
        }
    }
}