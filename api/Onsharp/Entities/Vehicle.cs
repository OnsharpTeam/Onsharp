using System;
using System.Drawing;
using Onsharp.Native;
using Onsharp.Utils;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Vehicle : LifelessEntity
    {
        /// <summary>
        /// The license plate of the vehicle.
        /// </summary>
        public string LicensePlate
        {
            get => Bridge.PtrToString(Onset.GetVehicleLicensePlate(Id));
            set => Onset.SetVehicleLicensePlate(Id, value);
        }

        /// <summary>
        /// The yaw rotation of the vehicle.
        /// </summary>
        public double Heading
        {
            get => Onset.GetVehicleHeading(Id);
            set => Onset.SetVehicleHeading(Id, value);
        }

        /// <summary>
        /// The health of the vehicle.
        /// </summary>
        public double Health
        {
            get => Onset.GetVehicleHealth(Id);
            set => Onset.SetVehicleHealth(Id, value);
        }
        
        /// <summary>
        /// The rotation of the vehicle.
        /// </summary>
        public Vector Rotation
        {
            get
            {
                double x = 0;
                double y = 0;
                double z = 0;
                Onset.GetVehicleRotation(Id, ref x, ref y, ref z);
                Vector vector = new Vector(x, y, z);
                vector.SyncCallback = () => Rotation = vector;
                return vector;
            }
            set => Onset.SetVehicleRotation(Id, value.X, value.Y, value.Z);
        }
        
        /// <summary>
        /// The velocity of the vehicle.
        /// </summary>
        public Vector Velocity
        {
            get
            {
                double x = 0;
                double y = 0;
                double z = 0;
                Onset.GetVehicleVelocity(Id, ref x, ref y, ref z);
                return new Vector(x, y, z);
            }
        }

        /// <summary>
        /// The color of vehicle.
        /// </summary>
        public Color Color
        {
            get => ColorUtils.HexToColor(Bridge.PtrToString(Onset.GetVehicleColor(Id)));
            set
            {
                string hex = ColorUtils.ColorToHex(value);
                Bridge.Logger.Debug("Veh Set Color : " + hex);
                Onset.SetVehicleColor(Id, hex);
            }
        }

        /// <summary>
        /// The trunk open ratio. If set to 0, the trunk is closed.
        /// </summary>
        public double TrunkRatio
        {
            get => Onset.GetVehicleTrunkRatio(Id);
            set => Onset.SetVehicleTrunkRatio(Id, value);
        }

        /// <summary>
        /// The hood open ratio. If set to 0, the hood is closed.
        /// </summary>
        public double HoodRatio
        {
            get => Onset.GetVehicleHoodRatio(Id);
            set => Onset.SetVehicleHoodRatio(Id, value);
        }

        /// <summary>
        /// Whether the lights are enabled or not.
        /// </summary>
        public bool IsLightEnabled
        {
            get => Onset.GetVehicleLightEnabled(Id);
            set => Onset.SetVehicleLightEnabled(Id, value);
        }

        /// <summary>
        /// The engine state of the vehicle.
        /// When true, the engine is on, otherwise its off.
        /// </summary>
        public bool IsEngineRunning
        {
            get => Onset.GetVehicleEngineState(Id);
            set
            {
                if (value)
                {
                    Onset.StartVehicleEngine(Id);
                }
                else
                {
                    Onset.StopVehicleEngine(Id);
                }
            }
        }
        
        /// <summary>
        /// The model of the vehicle.
        /// </summary>
        public int Model => Onset.GetVehicleModel(Id);

        /// <summary>
        /// The model name of the vehicle.
        /// </summary>
        public string ModelName => Bridge.PtrToString(Onset.GetVehicleModelName(Id));

        /// <summary>
        /// The maximal number of seats of the vehicle.
        /// </summary>
        public int MaxSeats => Onset.GetVehicleNumberOfSeats(Id);

        /// <summary>
        /// The current gear of the vehicle.
        /// </summary>
        public int Gear => Onset.GetVehicleGear(Id);

        /// <summary>
        /// The passenger of the given seat.
        /// </summary>
        /// <param name="seat">The wanted seat</param>
        public Player this[int seat]
        {
            get => GetPassenger(seat);
            set => SetPassenger(value, seat);
        }
        
        /// <summary>
        /// The driver of the vehicle, or null if the driver seat is not occupied.
        /// </summary>
        public Player Driver
        {
            get
            {
                int driver = Onset.GetVehicleDriver(Id);
                if (driver <= 0) return null;
                return Owner.CreatePlayer(driver);
            }
        }
        
        public Vehicle(int id) : base(id, "Vehicle")
        {
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
        /// Checks if the vehicle is streamed in for the player.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>True, if the vehicle is streamed in for the player</returns>
        public bool IsStreamedFor(Player player)
        {
            return Onset.IsStreamedIn(EntityName, player.Id, Id);
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
        /// Gets the damage of the given vehicle part.
        /// </summary>
        /// <param name="index">The index of the wanted vehicle part</param>
        /// <returns>True on success</returns>
        /// <exception cref="ArgumentException">If the index is out of range (between 1 and 8)</exception>
        public float GetDamage(byte index)
        {
            if(index < 1 || index > 8)
                throw new ArgumentException("The damage index cannot be greater than 8 and less than 1");
            return Onset.GetVehicleDamage(Id, index);
        }

        /// <summary>
        /// Sets the damage of the given vehicle part.
        /// </summary>
        /// <param name="index">The index of the wanted vehicle part</param>
        /// <param name="damage">The damage value to be set</param>
        /// <returns>True on success</returns>
        /// <exception cref="ArgumentException">If the index is out of range (between 1 and 8)</exception>
        public bool SetDamage(byte index, float damage)
        {
            if(index < 1 || index > 8)
                throw new ArgumentException("The damage index cannot be greater than 8 and less than 1");
            return Onset.SetVehicleDamage(Id, index, damage);
        }

        /// <summary>
        /// Sets the respawn parameters for the vehicle.
        /// </summary>
        /// <param name="enableRespawn">True enables the respawning of the vehicle</param>
        /// <param name="respawnTime">The time which defines the delay in milliseconds after which the vehicle gets respawned</param>
        /// <param name="repairOnRespawn">True enables the full repairing after respawn</param>
        /// <returns>True on success</returns>
        public bool SetRespawnParameters(bool enableRespawn, long respawnTime, bool repairOnRespawn = true)
        {
            return Onset.SetVehicleRespawnParams(Id, enableRespawn, respawnTime, repairOnRespawn);
        }

        /// <summary>
        /// Gets the passenger of the given seat, or null if the seat is not occupied.
        /// </summary>
        /// <param name="seat">The wanted seat</param>
        /// <returns>The seated player or null</returns>
        public Player GetPassenger(int seat)
        {
            int passenger = Onset.GetVehiclePassenger(Id, seat);
            if (passenger <= 0) return null;
            return Owner.CreatePlayer(passenger);
        }

        /// <summary>
        /// Sets the given passenger seat to the given player.
        /// </summary>
        /// <param name="player">The player to be set into the vehicle</param>
        /// <param name="seat">The wanted seat. Seat 1 is the driver seat</param>
        public void SetPassenger(Player player, int seat)
        {
            player.SetIntoVehicle(this, seat);
        }

        /// <summary>
        /// Sets the angular velocity of the vehicle.
        /// </summary>
        /// <param name="vector">The vector representing the wanted velocity</param>
        public void SetAngularVelocity(Vector vector)
        {
            Onset.SetVehicleAngularVelocity(Id, vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Sets the linear velocity of the vehicle.
        /// </summary>
        /// <param name="vector">The vector representing the wanted velocity</param>
        public void SetLinearVelocity(Vector vector)
        {
            Onset.SetVehicleLinearVelocity(Id, vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Attaches the nitro boost of the vehicle.
        /// </summary>
        public void AttachNitro()
        {
            Onset.AttachVehicleNitro(Id, true);
        }

        /// <summary>
        /// Detaches the nitro boost of the vehicle.
        /// </summary>
        public void DetachNitro()
        {
            Onset.AttachVehicleNitro(Id, false);
        }

        /// <summary>
        /// Enables the backfire effect of the vehicle.
        /// </summary>
        public void EnableBackfire()
        {
            Onset.EnableVehicleBackfire(Id, true);
        }

        /// <summary>
        /// Disables the backfire effect of the vehicle.
        /// </summary>
        public void DisableBackfire()
        {
            Onset.EnableVehicleBackfire(Id, false);
        }
    }
}