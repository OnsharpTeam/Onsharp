using System;
using System.Collections.Generic;
using System.Text;
using Onsharp.Enums;
using Onsharp.Events;
using Onsharp.Native;
using Onsharp.Steam;
using Onsharp.Utils;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Player : Entity
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name
        {
            get => Bridge.PtrToString(Onset.GetPlayerName(Id));
            set => Onset.SetPlayerName(Id, value);
        }

        /// <summary>
        /// The current network stats of the player.
        /// </summary>
        public NetworkStats NetworkStats => Bridge.GetNetworkStats(Id);

        /// <summary>
        /// A list containing all vehicles that are currently streamed for the player.
        /// </summary>
        public List<Vehicle> StreamedVehicles => Owner.Vehicles.SelectAll(vehicle => vehicle.IsStreamedFor(this));

        /// <summary>
        /// The steam of the player. Only available after the <see cref="EventType.PlayerSteamAuth"/> was called.
        /// </summary>
        public long SteamID => Onset.GetPlayerSteamId(Id);

        /// <summary>
        /// The steam id of the player as string and in the 64 format.
        /// </summary>
        public string SteamID64 => SteamID.ToString();

        /// <summary>
        /// The steam id of the player in the 32 format.
        /// </summary>
        public string SteamID32 => SteamIDConvert.Steam64ToSteam32(SteamID);

        /// <summary>
        /// The steam id of the player in the second format.
        /// </summary>
        public string SteamID2 => SteamIDConvert.Steam64ToSteam2(SteamID);
        
        /// <summary>
        /// Whether the voice is enabled and the player can talk in voice chat, or not.
        /// </summary>
        public bool IsVoiceEnabled
        {
            get => Onset.IsPlayerVoiceEnabled(Id);
            set => Onset.SetPlayerVoiceEnabled(Id, value);
        }

        /// <summary>
        /// Whether the player is talking or not.
        /// </summary>
        public bool IsTalking => Onset.IsPlayerTalking(Id);

        /// <summary>
        /// The state of the player.
        /// </summary>
        public PlayerState State => (PlayerState) Onset.GetPlayerState(Id);

        /// <summary>
        /// The movement mode of the player.
        /// </summary>
        public MoveMode MoveMode => (MoveMode) Onset.GetPlayerMovementMode(Id);

        /// <summary>
        /// The current movement speed of the player.
        /// </summary>
        public double Speed => Onset.GetPlayerMovementSpeed(Id);

        /// <summary>
        /// Whether the player is aiming with a weapon or not.
        /// </summary>
        public bool IsAiming => Onset.IsPlayerAiming(Id);

        /// <summary>
        /// Whether the player is reloading a weapon or not.
        /// </summary>
        public bool IsReloading => Onset.IsPlayerReloading(Id);

        /// <summary>
        /// The vehicle the player is currently sitting in, null if he is not in a vehicle currently.
        /// </summary>
        public Vehicle Vehicle
        {
            get
            {
                int vehicleId = Onset.GetPlayerVehicle(Id);
                if (vehicleId <= 0) return null;
                return Owner.CreateVehicle(vehicleId);
            }
        }

        /// <summary>
        /// Whether the player is in a vehicle or not.
        /// </summary>
        public bool IsInVehicle => Onset.GetPlayerVehicle(Id) > 0;

        /// <summary>
        /// The seat the player is currently sitting on, if the player is sitting in a vehicle.
        /// </summary>
        public int VehicleSeat => Onset.GetPlayerVehicleSeat(Id);

        /// <summary>
        /// The slot the player has currently selected.
        /// </summary>
        public int SelectedSlot => Onset.GetPlayerEquippedWeaponSlot(Id);

        /// <summary>
        /// Whether the player is dead or not.
        /// </summary>
        public bool IsDead => Onset.IsPlayerDead(Id);

        /// <summary>
        /// The yaw rotation of the player.
        /// </summary>
        public double Heading
        {
            get => Onset.GetPlayerHeading(Id);
            set => Onset.SetPlayerHeading(Id, value);
        }

        /// <summary>
        /// The health of the player.
        /// </summary>
        public double Health
        {
            get => Onset.GetPlayerHealth(Id);
            set => Onset.SetPlayerHealth(Id, value);
        }

        /// <summary>
        /// The armor of the player.
        /// </summary>
        public double Armor
        {
            get => Onset.GetPlayerArmor(Id);
            set => Onset.SetPlayerArmor(Id, value);
        }

        /// <summary>
        /// The head size of the player.
        /// </summary>
        public float HeadSize
        {
            get => Onset.GetPlayerHeadSize(Id);
            set
            {
                if(value < 0 || value > 3)
                    throw new ArgumentException("The slot cannot be less than 0 and greater than 3!");
                Onset.SetPlayerHeadSize(Id, value);
            }
        }

        /// <summary>
        /// The respawn time of the player in milliseconds.
        /// </summary>
        public long RespawnTime
        {
            get => Onset.GetPlayerRespawnTime(Id);
            set => Onset.SetPlayerRespawnTime(Id, value);
        }

        /// <summary>
        /// The internet IP of the player.
        /// </summary>
        public string IP => Bridge.PtrToString(Onset.GetPlayerIP(Id));

        /// <summary>
        /// The ping of the player.
        /// </summary>
        public int Ping => Onset.GetPlayerPing(Id);

        /// <summary>
        /// The game version of the player.
        /// </summary>
        public int GameVersion => Onset.GetPlayerGameVersion(Id);
        
        /// <summary>
        /// The player's game locale.
        /// </summary>
        public string Locale => Bridge.PtrToString(Onset.GetPlayerLocale(Id));
        
        /// <summary>
        /// The player's system's GUID.
        /// </summary>
        public string GUID => Bridge.PtrToString(Onset.GetPlayerGUID(Id));
 
        /// <summary>
        /// The status of the player in association of the given voice channel.
        /// <br/><see cref="IsInVoiceChannel"/>
        /// <br/><see cref="AddToVoiceChannel"/>
        /// <br/><see cref="RemoveFromVoiceChannel"/>
        /// </summary>
        /// <param name="voiceChannel">The wanted voice channel</param>
        public bool this[int voiceChannel]
        {
            get => IsInVoiceChannel(voiceChannel);
            set
            {
                if (value)
                {
                    AddToVoiceChannel(voiceChannel);
                }
                else
                {
                    RemoveFromVoiceChannel(voiceChannel);
                }
            }
        }
        
        /// <summary>
        /// Whether this player is admin or not. If set to true, the player has administrator permission which means he can run all commands on the server.
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// The permissions the player has. Permissions can be wildcarded.
        /// </summary>
        private readonly List<string> _permissions;

        public Player(int id) : base(id, "Player")
        {
            _permissions = new List<string>();
        }

        /// <summary>
        /// Adds the given permission to the player.
        /// </summary>
        /// <param name="permission">The permission to be added</param>
        public void AddPermission(string permission)
        {
            lock (_permissions)
            {
                _permissions.Add(permission);
            }
        }
        
        /// <summary>
        /// Removes the given permission from the player.
        /// </summary>
        /// <param name="permission">The permission to be removed</param>
        public void RemovePermission(string permission)
        {
            lock (_permissions)
            {
                _permissions.Remove(permission);
            }
        }

        /// <summary>
        /// Checks if the player has the given permission or not.
        /// </summary>
        /// <param name="permission">The permission to be checked</param>
        /// <returns>True if the player has the permission</returns>
        public bool HasPermission(string permission)
        {
            if (IsAdmin) return true;
            lock (_permissions)
            {
                if (_permissions.Contains("*")) return true;
                if (_permissions.Contains(permission)) return true;
                string[] parts = permission.Split('.');
                string lastPart = "";
                foreach (string part in parts)
                {
                    lastPart += part + ".";
                    if (_permissions.Contains(lastPart + "*")) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Sends a message to the player's chat.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="args">The arguments which should be passed to the string</param>
        public void SendMessage(string message, params object[] args)
        {
            string formattedString = string.Format(message, args);
            Onset.SendPlayerChatMessage(Id, formattedString);
        }

        /// <summary>
        /// Calls a remote event handler on the client-side of this player.
        /// </summary>
        /// <param name="name">The name of the remote event handler which should be called</param>
        /// <param name="args">The arguments which will passed to the event handler. The maximum length is 10</param>
        public void CallRemote(string name, params object[] args)
        {
            if(args.Length > 10)
                throw new ArgumentException("The maximum length of event handler arguments are 10!");

            IntPtr[] argsArr = new IntPtr[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                object arg = args[i];
                if (arg is Entity entity)
                {
                    arg = entity.Id;
                }
                
                argsArr[i] = Bridge.CreateNValue(arg).NativePtr;
            }
            
            Onset.CallRemote(Id, name, argsArr, argsArr.Length);
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
        /// Sets the visibility of the given pickup for the player.
        /// </summary>
        /// <param name="pickup">The pickup for which the visibility should be changed</param>
        /// <param name="visible">True, if the pickup should be visible</param>
        public void SetPickupVisibility(Pickup pickup, bool visible)
        {
            Onset.SetPickupVisibility(pickup.Id, Id, visible);
        }

        /// <summary>
        /// Sets the visibility of the given 3D text for the player.
        /// </summary>
        /// <param name="text3d">The 3D text for which the visibility should be changed</param>
        /// <param name="visible">True, if the pickup should be visible</param>
        public void SetText3DVisibility(Text3D text3d, bool visible)
        {
            Onset.SetText3DVisibility(text3d.Id, Id, visible);
        }

        /// <summary>
        /// Checks if the player is streamed in for the given player.
        /// </summary>
        /// <param name="player">The given player</param>
        /// <returns>True, if the player is streamed in for the given player</returns>
        public bool IsStreamedFor(Player player)
        {
            return Onset.IsStreamedIn(EntityName, player.Id, Id);
        }

        /// <summary>
        /// Checks if the given player is streamed in for the player.
        /// </summary>
        /// <param name="player">The given player</param>
        /// <returns>True, if the given player is streamed in for the player</returns>
        public bool IsStreamedIn(Player player)
        {
            return player.IsStreamedFor(this);
        }

        /// <summary>
        /// Checks if the given vehicle is streamed in for the player.
        /// </summary>
        /// <param name="vehicle">The given vehicle</param>
        /// <returns>True, if the given vehicle is streamed in for the player</returns>
        public bool IsStreamedIn(Vehicle vehicle)
        {
            return vehicle.IsStreamedFor(this);
        }

        /// <summary>
        /// Checks if the given object is streamed in for the player.
        /// </summary>
        /// <param name="obj">The given object</param>
        /// <returns>True, if the given object is streamed in for the player</returns>
        public bool IsStreamedIn(Object obj)
        {
            return obj.IsStreamedFor(this);
        }

        /// <summary>
        /// Checks if the given NPC is streamed in for the player.
        /// </summary>
        /// <param name="npc">The given NPC</param>
        /// <returns>True, if the given NPC is streamed in for the player</returns>
        public bool IsStreamedIn(NPC npc)
        {
            return npc.IsStreamedFor(this);
        }

        /// <summary>
        /// Sets the spawn location of the player.
        /// </summary>
        /// <param name="pos">The location to be set to</param>
        /// <param name="heading">The yaw rotation to be set to</param>
        public void SetSpawnLocation(Vector pos, double heading = 0)
        {
            Onset.SetPlayerSpawnLocation(Id, pos.X, pos.Y, pos.Z, heading);
        }

        /// <summary>
        /// Enables the given voice channel for the player.
        /// </summary>
        /// <param name="channel">The voice channel to be changed</param>
        public void AddToVoiceChannel(int channel)
        {
            Onset.SetPlayerVoiceChannel(Id, channel, true);
        }

        /// <summary>
        /// Disables the given voice channel for the player.
        /// </summary>
        /// <param name="channel">The voice channel to be changed</param>
        public void RemoveFromVoiceChannel(int channel)
        {
            Onset.SetPlayerVoiceChannel(Id, channel, false);
        }

        /// <summary>
        /// Checks if the player is in the given voice channel.
        /// </summary>
        /// <param name="channel">The voice channel to be checked</param>
        /// <returns>True, if the player is in the given voice channel</returns>
        public bool IsInVoiceChannel(int channel)
        {
            return Onset.IsPlayerVoiceChannel(Id, channel);
        }

        /// <summary>
        /// Sets the dimension of the player's voice.
        /// </summary>
        /// <param name="dim">The dimension to be set to</param>
        /// <returns>True on success</returns>
        public bool SetVoiceDimension(uint dim)
        {
            return Onset.SetPlayerVoiceDimension(Id, dim);
        }

        /// <summary>
        /// Sets the player into the given vehicle on the given seat.
        /// </summary>
        /// <param name="vehicle">The vehicle to be entered</param>
        /// <param name="seat">The wanted seat</param>
        public void SetIntoVehicle(Vehicle vehicle, int seat = 0)
        {
            Onset.SetPlayerInVehicle(Id, vehicle.Id, seat);
        }

        /// <summary>
        /// Forces the player to leave the current vehicle, if he is sitting in one.
        /// </summary>
        public void LeaveVehicle()
        {
            Onset.RemovePlayerFromVehicle(Id);
        }

        /// <summary>
        /// Sets the wanted weapon stat for the player for the given weapon.
        /// </summary>
        /// <param name="weapon">The given weapon</param>
        /// <param name="stat">The stat to be changed</param>
        /// <param name="value">The value to which it will be changed</param>
        /// <returns>True on success</returns>
        public bool SetWeaponStat(Weapon weapon, WeaponStat stat, double value)
        {
            return Onset.SetPlayerWeaponStat(Id, (int) weapon, stat.GetName(), value);
        }

        /// <summary>
        /// Gives the player the given weapon on the given slot.
        /// </summary>
        /// <param name="slot">The slot to be set</param>
        /// <param name="weapon">The weapon to be set to</param>
        /// <param name="ammo">The ammo of the weapon</param>
        /// <param name="equip">True, if the slot should be equipped automatically</param>
        /// <param name="loaded">True, if the weapon should be loaded automatically</param>
        /// <returns>True on success</returns>
        /// <exception cref="ArgumentException">When the slot is out of range (between 1 and 3)</exception>
        public bool GiveWeapon(int slot, Weapon weapon, int ammo = 1, bool equip = true, bool loaded = true)
        {
            if(slot < 1 || slot > 3)
                throw new ArgumentException("The slot cannot be less than 1 and greater than 3!");
            return Onset.SetPlayerWeapon(Id, (int) weapon, ammo, equip, slot, loaded);
        }

        /// <summary>
        /// Gets the currently equipped weapon on the given slot of the player.
        /// </summary>
        /// <param name="slot">The wanted slot</param>
        /// <param name="ammo">The ammo currently in the weapon</param>
        /// <returns>The equipped weapon</returns>
        /// <exception cref="ArgumentException">When the slot is out of range (between 1 and 3)</exception>
        public Weapon GetCurrentWeapon(int slot, out int ammo)
        {
            if(slot < 1 || slot > 3)
                throw new ArgumentException("The slot cannot be less than 1 and greater than 3!");
            int model = 0, rAmmo = 0;
            Onset.GetPlayerWeapon(Id, slot, ref model, ref rAmmo);
            ammo = rAmmo;
            return (Weapon) model;
        }

        /// <summary>
        /// Gets the ammo of the currently equipped weapon on the given slot of the player.
        /// </summary>
        /// <param name="slot">The wanted slot</param>
        /// <returns>The ammo of the equipped weapon</returns>
        /// <exception cref="ArgumentException">When the slot is out of range (between 1 and 3)</exception>
        public int GetCurrentAmmo(int slot)
        {
            if(slot < 1 || slot > 3)
                throw new ArgumentException("The slot cannot be less than 1 and greater than 3!");
            int model = 0, rAmmo = 0;
            Onset.GetPlayerWeapon(Id, slot, ref model, ref rAmmo);
            return rAmmo;
        }

        /// <summary>
        /// Gets the currently equipped weapon on the given slot of the player.
        /// </summary>
        /// <param name="slot">The wanted slot</param>
        /// <returns>The equipped weapon</returns>
        /// <exception cref="ArgumentException">When the slot is out of range (between 1 and 3)</exception>
        public Weapon GetCurrentWeapon(int slot)
        {
            if(slot < 1 || slot > 3)
                throw new ArgumentException("The slot cannot be less than 1 and greater than 3!");
            int model = 0, rAmmo = 0;
            Onset.GetPlayerWeapon(Id, slot, ref model, ref rAmmo);
            return (Weapon) model;
        }

        /// <summary>
        /// Changes the selected slot of the player to the given slot.
        /// </summary>
        /// <param name="slot">The slot to be changed to</param>
        /// <returns>True on success</returns>
        /// <exception cref="ArgumentException">When the slot is out of range (between 1 and 3)</exception>
        public bool ChangeSelectedSlot(int slot)
        {
            if(slot < 1 || slot > 3)
                throw new ArgumentException("The slot cannot be less than 1 and greater than 3!");
            return Onset.EquipPlayerWeaponSlot(Id, slot);
        }

        /// <summary>
        /// Enables the spectate mode.
        /// </summary>
        public void EnableSpectate()
        {
            Onset.SetPlayerSpectate(Id, true);
        }

        /// <summary>
        /// Disables the spectate mode.
        /// </summary>
        public void DisableSpectate()
        {
            Onset.SetPlayerSpectate(Id, false);
        }

        /// <summary>
        /// Kicks the player with the given reason.
        /// </summary>
        /// <param name="reason">The kick message</param>
        public void Kick(string reason = "")
        {
            Onset.KickPlayer(Id, reason);
        }

        /// <summary>
        /// Plays an animation for the player.
        /// </summary>
        /// <param name="animation">The animation to be played</param>
        public void PlayAnimation(Animation animation)
        {
            Onset.SetPlayerAnimation(Id, animation.GetName());
        }

        /// <summary>
        /// Attaches the parachute of the player.
        /// </summary>
        public void AttachParachute()
        {
            Onset.AttachPlayerParachute(Id, true);
        }

        /// <summary>
        /// Detaches the parachute of the player.
        /// </summary>
        public void DetachParachute()
        {
            Onset.AttachPlayerParachute(Id, false);
        }

        /// <summary>
        /// Sets if the player is in ragdoll or not.
        /// </summary>
        /// <param name="enable">True, if the player should be in ragdoll</param>
        public void SetRagdoll(bool enable)
        {
            Onset.SetPlayerRagdoll(Id, enable);
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void StopAnimation()
        {
            PlayAnimation(Animation.Stop);
        }

        #region Colored Chat

        /// <summary>
        /// Sends a colored message to the player's chat.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="size">The size of the font</param>
        public void SendColoredMessage(string message, int size = 10)
        {
            StringBuilder formattedMessage = new StringBuilder();
            string currentPart = "<span size=\"" + size + "\">";
            for(int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if (c == '~')
                {
                    formattedMessage.Append(currentPart + "</>");
                    i++;
                    string color = GetColorCode(message[i]);
                    i++;
                    c = message[i];
                    string style = null;
                    if (c != '~')
                    {
                        style = GetStyleCode(c);
                        i++;
                    }

                    currentPart = "<span size=\"" + size + "\" " + (color != null ? "color=\"#" + color + "\"" : "") +
                                  (style != null ? " style=\"" + style + "\"" : "") + ">";
                }
                else
                {
                    currentPart += c;
                }
            }
            

            formattedMessage.Append(currentPart + "</>");
            SendMessage(formattedMessage.ToString());
        }

        private string GetStyleCode(char c)
        {
            switch (c)
            {
                case '*':
                    return "bold";
                case '/':
                    return "italic";
                default:
                    return null;
            }
        }

        private string GetColorCode(char c)
        {
            switch (c)
            {
                case '0':
                    return "000000";
                case '1':
                    return "0000AA";
                case '2':
                    return "00AA00";
                case '3':
                    return "00AAAA";
                case '4':
                    return "AA0000";
                case '5':
                    return "AA00AA";
                case '6':
                    return "FFAA00";
                case '7':
                    return "AAAAAA";
                case '8':
                    return "555555";
                case '9':
                    return "5555FF";
                case 'a':
                    return "55FF55";
                case 'b':
                    return "55FFFF";
                case 'c':
                    return "FF5555";
                case 'd':
                    return "FF55FF";
                case 'e':
                    return "FFFF55";
                case 'f':
                case 'r':
                    return "FFFFFF";
                default:
                    return null;
                
            }
        }

        #endregion
    }
}