using Onsharp.Entities;

namespace TestPlugin
{
    public class MyPlayer : Player
    {
        public int MyID { get; }

        public MyPlayer(int id) : base(id)
        {
            MyID = PluginMain.Random.Next(10);
        }
    }
}