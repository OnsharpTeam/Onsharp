using Onsharp.Entities;
using Onsharp.World;

namespace Onsharp.Events
{
   /// <summary>
    /// All event types which can be listened to.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// The custom event can be called by a specified name and must be called manually.
        /// </summary>
        Custom = -1,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> quits the server.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player)
        /// </summary>
        PlayerQuit = 0,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> writes something in the chat.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="string"/> message)
        /// </summary> 
        PlayerChat = 1,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> starts executing a command.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="string"/> command, <see cref="bool"/> exists)
        /// </summary>
        PlayerChatCommand = 2,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> joins the server.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player)
        /// </summary>
        PlayerJoin = 3,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> pickups a <see cref="Onsharp.Entities.Pickup"/>.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Pickup"/> pickup)
        /// </summary>
        PlayerPickupHit = 4,
        /// <summary>
        /// Called on execution of the main thread.<br/>
        /// (<see cref="float"/> deltaSeconds)
        /// </summary>
        GameTick = 8,
        /// <summary>
        /// Called when a client tries to connect to the server.<br/>
        /// (<see cref="string"/> ip, <see cref="int"/> port)<br/>
        /// <returns>Returning false results in denying the connection request and kicking the client</returns>
        /// </summary>
        ClientConnectionRequest = 9,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> reached its target.<br/>
        /// (<see cref="Onsharp.Entities.NPC"/> npc)
        /// </summary>
        NPCReachTarget = 10,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> is damaged.<br/>
        /// (<see cref="Onsharp.Entities.NPC"/> npc, <see cref="Onsharp.Enums.DamageType"/> damageType, <see cref="double"/> amount)
        /// </summary>
        NPCDamage = 11,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> is spawned.<br/>
        /// (<see cref="Onsharp.Entities.NPC"/> npc)
        /// </summary>
        NPCSpawn = 12,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> dies.<br/>
        /// (<see cref="Onsharp.Entities.NPC"/> npc)
        /// </summary>
        NPCDeath = 13,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> is streamed for a player.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.NPC"/> npc)
        /// </summary>
        NPCStreamIn = 14,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> is no longer streamed for a player.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.NPC"/> npc)
        /// </summary>
        NPCStreamOut = 15,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> enters a <see cref="Onsharp.Entities.Vehicle"/>.
        /// The seat is either the driver seat (0) or the passenger seats.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Vehicle"/> vehicle, <see cref="int"/> seat)
        /// </summary>
        PlayerEnterVehicle = 16,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> leaves a <see cref="Onsharp.Entities.Vehicle"/>.
        /// The seat is either the driver seat (0) or the passenger seats.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Vehicle"/> vehicle, <see cref="int"/> seat)
        /// </summary>
        PlayerLeaveVehicle = 17,
        /// <summary>
        /// Called when a <see cref="Onsharp.Enums.PlayerState"/> changes for a <see cref="Onsharp.Entities.Player"/>.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Enums.PlayerState"/> newState, <see cref="Onsharp.Enums.PlayerState"/> oldState)
        /// </summary>
        PlayerStateChange = 18,
        /// <summary>
        /// Called when a vehicle respawns.<br/>
        /// (<see cref="Onsharp.Entities.Vehicle"/> vehicle)
        /// </summary>
        VehicleRespawn = 19,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Vehicle"/> is streamed for a player.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Vehicle"/> vehicle)
        /// </summary>
        VehicleStreamIn = 20,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Vehicle"/> is no longer streamed for a player.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Vehicle"/> vehicle)
        /// </summary>
        VehicleStreamOut = 21,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> has been authorized by the server and is called after <see cref="ClientConnectionRequest"/> and before <see cref="PlayerJoin"/><br/>
        /// (<see cref="Onsharp.Entities.Player"/> player)
        /// </summary>
        PlayerServerAuth = 22,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> has been authorized by Steam. After this call <see cref="Onsharp.Entities.Player.SteamID"/> is available.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player)
        /// </summary>
        PlayerSteamAuth = 23,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> finished downloading.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="string"/> fileName, <see cref="string"/> checksum)
        /// </summary>
        PlayerDownloadFile = 24,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> is streamed for a player.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Player"/> other)
        /// </summary>
        PlayerStreamIn = 25,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> is no longer streamed for a player.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Player"/> other)
        /// </summary>
        PlayerStreamOut = 26,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> is respawning.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player)
        /// </summary>
        PlayerSpawn = 27,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> dies.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Player"/> killer)
        /// </summary>
        PlayerDeath = 28,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> has shot their weapon.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Enums.Weapon"/> weapon, <see cref="Onsharp.Enums.HitType"/> hitType, <see cref="Onsharp.Entities.Entity"/> target, <see cref="Vector"/> hitPos, <see cref="Vector"/> startPos, <see cref="Vector"/> impactPos)<br/>
        /// <returns>Returning false results in preventing the hit from further processing</returns>
        /// </summary>
        PlayerWeaponShot = 29,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> is damaged.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Enums.DamageType"/> damageType, <see cref="double"/> amount)
        /// </summary>
        PlayerDamage = 30,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> interacts with a door.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.Entities.Door"/> door, <see cref="bool"/> isBeingOpened)
        /// </summary>
        PlayerInteractDoor = 31,
        /// <summary>
        /// Called before the command is being processed.<br/>
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="string"/> commandName, <see cref="string"/>[] args)
        /// /// <returns>Returning false results in preventing the further processing</returns>
        /// </summary>
        PlayerPreCommand = 32,
        /// <summary>
        /// Called when a vehicle takes health or physical damage.<br/>
        /// (<see cref="Onsharp.Entities.Vehicle"/> vehicle, <see cref="double"/> healthDamage, <see cref="int"/>damageIndex, <see cref="double"/> physicalDamage)
        /// </summary>
        VehicleDamage = 33,
        /// <summary>
        /// Called when a user enters text in the console and hits enter.
        /// (<see cref="string"/>)
        /// </summary>
        ConsoleInput = 34,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Door"/> has been destroyed.
        /// (<see cref="Onsharp.Entities.Door"/>)
        /// </summary>
        DoorDestroyed = 35,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> has been destroyed.
        /// (<see cref="Onsharp.Entities.NPC"/>)
        /// </summary>
        NPCDestroyed = 36,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Object"/> has been destroyed.
        /// (<see cref="Onsharp.Entities.Object"/>)
        /// </summary>
        ObjectDestroyed = 37,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Pickup"/> has been destroyed.
        /// (<see cref="Onsharp.Entities.Pickup"/>)
        /// </summary>
        PickupDestroyed = 38,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Text3D"/> has been destroyed.
        /// (<see cref="Onsharp.Entities.Text3D"/>)
        /// </summary>
        Text3DDestroyed = 39,
        //----- NEW EVENTS ----\\
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Player"/> changed the dimension.
        /// (<see cref="Onsharp.Entities.Player"/> player, <see cref="Onsharp.World.Dimension"/> old, <see cref="Onsharp.World.Dimension"/> new)
        /// </summary>
        PlayerChangeDimension = 40,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Vehicle"/> changed the dimension.
        /// (<see cref="Onsharp.Entities.Vehicle"/> vehicle, <see cref="Onsharp.World.Dimension"/> old, <see cref="Onsharp.World.Dimension"/> new)
        /// </summary>
        VehicleChangeDimension = 41,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Text3D"/> changed the dimension.
        /// (<see cref="Onsharp.Entities.Text3D"/> tex3D, <see cref="Onsharp.World.Dimension"/> old, <see cref="Onsharp.World.Dimension"/> new)
        /// </summary>
        Text3DChangeDimension = 42,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Pickup"/> changed the dimension.
        /// (<see cref="Onsharp.Entities.Pickup"/> pickup, <see cref="Onsharp.World.Dimension"/> old, <see cref="Onsharp.World.Dimension"/> new)
        /// </summary>
        PickupChangeDimension = 43,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Object"/> changed the dimension.
        /// (<see cref="Onsharp.Entities.Object"/> obj, <see cref="Onsharp.World.Dimension"/> old, <see cref="Onsharp.World.Dimension"/> new)
        /// </summary>
        ObjectChangeDimension = 44,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> changed the dimension.
        /// (<see cref="Onsharp.Entities.NPC"/> npc, <see cref="Onsharp.World.Dimension"/> old, <see cref="Onsharp.World.Dimension"/> new)
        /// </summary>
        NPCChangeDimension = 45,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Object"/> got created.
        /// (<see cref="Onsharp.Entities.Object"/>)
        /// </summary>
        ObjectCreated = 46,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Vehicle"/> got created.
        /// (<see cref="Onsharp.Entities.Vehicle"/>)
        /// </summary>
        VehicleCreated = 47,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Text3D"/> got created.
        /// (<see cref="Onsharp.Entities.Text3D"/>)
        /// </summary>
        Text3DCreated = 48,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Pickup"/> got created.
        /// (<see cref="Onsharp.Entities.Pickup"/>)
        /// </summary>
        PickupCreated = 49,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.NPC"/> got created.
        /// (<see cref="Onsharp.Entities.NPC"/>)
        /// </summary>
        NPCCreated = 50,
        /// <summary>
        /// Called when a <see cref="Onsharp.Entities.Door"/> got created.
        /// (<see cref="Onsharp.Entities.Door"/>)
        /// </summary>
        DoorCreated = 51,
        /// <summary>
        /// Called when an <see cref="Onsharp.Entities.Object"/> stopped moving after <see cref="Object.MoveTo"/>.
        /// (<see cref="Onsharp.Entities.Object"/>)
        /// </summary>
        ObjectStopMoving = 52
    }
}