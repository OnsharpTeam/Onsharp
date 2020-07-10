using Onsharp.Native;

namespace Onsharp.Entities
{
    public class Door : LifelessEntity
    {
        /// <summary>
        /// Whether the door is open or not.
        /// </summary>
        public bool Open
        {
            get => Onset.GetDoorOpen(Id);
            set => Onset.SetDoorOpen(Id, value);
        }

        /// <summary>
        /// The model of the door.
        /// </summary>
        public int Model => Onset.GetDoorModel(Id);
        
        public Door(int id) : base(id, "Door")
        {
        }
    }
}