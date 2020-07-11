using Onsharp.Enums;
using Onsharp.Native;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class NPC : LifelessEntity
    {
        /// <summary>
        /// The health of the NPC.
        /// </summary>
        public double Health
        {
            get => Onset.GetNPCHealth(Id);
            set => Onset.SetNPCHealth(Id, value);
        }

        /// <summary>
        /// The yaw rotation of the NPC.
        /// </summary>
        public double Heading
        {
            get => Onset.GetNPCHeading(Id);
            set => Onset.SetNPCHeading(Id, value);
        }

        public NPC(int id) : base(id, "NPC")
        {
        }

        /// <summary>
        /// Sets if the NPC is in ragdoll or not.
        /// </summary>
        /// <param name="enable">True, if the NPC should be in ragdoll</param>
        public void SetRagdoll(bool enable)
        {
            Onset.SetNPCRagdoll(Id, enable);
        }

        /// <summary>
        /// Checks if the NPC is streamed in for the player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>True, if the NPC is streamed in for the player</returns>
        public bool IsStreamedFor(Player player)
        {
            return Onset.IsStreamedIn(EntityName, player.Id, Id);
        }

        /// <summary>
        /// Plays an animation for the NPC.
        /// </summary>
        /// <param name="animation">The animation to be played</param>
        /// <param name="loop">True, if the animation should loop</param>
        public void PlayAnimation(Animation animation, bool loop = false)
        {
            Onset.SetNPCAnimation(Id, animation.GetName(), loop);
        }

        /// <summary>
        /// Forces the NPC to move to the given position.
        /// </summary>
        /// <param name="pos">The position the NPC should move to</param>
        /// <param name="speed">The speed in which the NPC walks or runs to the position</param>
        public void MoveTo(Vector pos, double speed = 160)
        {
            Onset.SetNPCTargetLocation(Id, pos.X, pos.Y, pos.Z, speed);
        }

        /// <summary>
        /// Forces the NPC to follow the given player.
        /// </summary>
        /// <param name="player">The player the NPC should follow</param>
        /// <param name="speed">The speed in which the NPC follows the player</param>
        public void Follow(Player player, double speed = 160)
        {
            Onset.SetNPCFollowPlayer(Id, player.Id, speed);   
        }
        /// <summary>
        /// Forces the NPC to follow the given vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle the NPC should follow</param>
        /// <param name="speed">The speed in which the NPC follows the vehicle</param>
        public void Follow(Vehicle vehicle, double speed = 160)
        {
            Onset.SetNPCFollowVehicle(Id, vehicle.Id, speed);   
        }
        
        /// <summary>
        /// Attaches the given object to the entity.
        /// </summary>
        /// <param name="obj">The object which gets attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Object obj, Vector pos, Vector rot, string socketName = "")
        {
            obj.Attach(this, pos, rot, socketName);
        }
        
        /// <summary>
        /// Attaches the given object to the entity.
        /// </summary>
        /// <param name="text3d">The object which gets attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Text3D text3d, Vector pos, Vector rot, string socketName = "")
        {
            text3d.Attach(this, pos, rot, socketName);
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void StopAnimation()
        {
            PlayAnimation(Animation.Stop);
        }
    }
}