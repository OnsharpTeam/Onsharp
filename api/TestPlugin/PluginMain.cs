using System;
using Onsharp.Commands;
using Onsharp.Entities;
using Onsharp.Events;
using Onsharp.Plugins;
using Onsharp.Updater;
using Onsharp.World;

namespace TestPlugin
{
    [AutoUpdater("https://eternitylife.de/test.json")]
    [PluginMeta("test-plugin", "TestPlugin", "1.1", "OnsharpTeam", IsDebug = true)]
    public class PluginMain : Plugin
    {
        public static readonly Random Random = new Random();
        
        public override void OnStart()
        {
            var config = Data.Config<MyConfig>();
            Logger.SetDebug(config.IsDebug);
            Logger.Debug("Debug mode is {STATE}!", "enabled");
            Server.OverrideEntityFactory(new MyPlayerFactory());
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

        [ServerEvent(EventType.PlayerSpawn)]
        public void OnPlayerSpawn(MyPlayer player)
        {
            Logger.Debug("Player has got Number " + player.MyID + " -> " + player.Name);
            player.SetPosition(125773.000000, 80246.000000, 1645.000000);
        }

        [Command("hello")]
        public void OnHelloCommand(Player player)
        {
            player.SendMessage("Hallo, " + player.Name + "!");
        }

        [Command("obj")]
        public void OnObjCommand(Player player)
        {
            Vector pos = player.GetPosition();
            Runtime.CreateObject(1, pos.X, pos.Y, pos.Z, 0, 0, 0, 1, 1, 1);
        }
    }
}