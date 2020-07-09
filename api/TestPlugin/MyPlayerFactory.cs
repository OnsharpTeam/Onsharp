using Onsharp.Entities.Factory;

namespace TestPlugin
{
    public class MyPlayerFactory : IEntityFactory<MyPlayer>
    {
        public MyPlayer Create(int id)
        {
            return new MyPlayer(id);
        }
    }
}