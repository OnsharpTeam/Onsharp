using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Onsharp.Native
{
    /// <summary>
    /// This is a collection of all native methods which will call them in the c++ runtime.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class Onset
    {
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPropertyValue([MarshalAs(UnmanagedType.LPStr)] string entityName, int entity,
            [MarshalAs(UnmanagedType.LPStr)] string propertyName);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPropertyValue([MarshalAs(UnmanagedType.LPStr)] string entityName, int entity,
            [MarshalAs(UnmanagedType.LPStr)] string propertyName, IntPtr propertyValue, bool sync);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetPlayerRagdoll(int player, bool enable);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long GetPlayerSteamId(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern float GetPlayerHeadSize(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerHeadSize(int player, float size);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void AttachPlayerParachute(int player, bool attach);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerAnimation(int player, [MarshalAs(UnmanagedType.LPStr)] string animation);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerGameVersion(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerGUID(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerLocale(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void KickPlayer(int player, [MarshalAs(UnmanagedType.LPStr)] string reason);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerPing(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerIP(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern long GetPlayerRespawnTime(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerRespawnTime(int player, long ms);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetPlayerArmor(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerArmor(int player, double armor);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetPlayerHealth(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerHealth(int player, double health);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPlayerDead(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerSpectate(int player, bool spectate);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetPlayerHeading(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerHeading(int player, double heading);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool EquipPlayerWeaponSlot(int player, int slot);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerEquippedWeaponSlot(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetPlayerWeapon(int player, int slot, ref int model, ref int ammo);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetPlayerWeapon(int player, int weapon, int ammo, bool equip, int slot, bool loaded);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetPlayerWeaponStat(int player, int weapon,
            [MarshalAs(UnmanagedType.LPStr)] string stat, double value);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RemovePlayerFromVehicle(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerInVehicle(int player, int vehicle, int seat);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerVehicleSeat(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerVehicle(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPlayerReloading(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPlayerAiming(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetPlayerMovementSpeed(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerMovementMode(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPlayerState(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetPlayerVoiceDimension(int player, uint dim);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPlayerTalking(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPlayerVoiceEnabled(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerVoiceEnabled(int player, bool enable);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPlayerVoiceChannel(int player, int channel);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerVoiceChannel(int player, int channel, bool enable);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerSpawnLocation(int player, double x, double y, double z, double heading);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EnableVehicleBackfire(int vehicle, bool enable);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void AttachVehicleNitro(int vehicle, bool attach);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetVehicleLightEnabled(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleLightEnabled(int vehicle, bool enabled);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetVehicleEngineState(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopVehicleEngine(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartVehicleEngine(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetVehicleTrunkRatio(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleTrunkRatio(int vehicle, double ratio);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetVehicleHoodRatio(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleHoodRatio(int vehicle, double ratio);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVehicleGear(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleAngularVelocity(int vehicle, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleLinearVelocity(int vehicle, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleColor(int vehicle, [MarshalAs(UnmanagedType.LPStr)] string hexColor);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetVehicleColor(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVehicleNumberOfSeats(int vehicle);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVehiclePassenger(int vehicle, int seat);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVehicleDriver(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetVehicleVelocity(int vehicle, ref double x, ref double y, ref double z);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleHealth(int vehicle, double health);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetVehicleHealth(int vehicle);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleHeading(int vehicle, double heading);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetVehicleHeading(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetVehicleRotation(int vehicle, ref double x, ref double y, ref double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleRotation(int vehicle, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetVehicleRespawnParams(int vehicle, bool enableRespawn, long respawnTime, bool repairOnRespawn);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetVehicleModelName(int vehicle);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVehicleModel(int vehicle);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetVehicleLicensePlate(int vehicle, [MarshalAs(UnmanagedType.LPStr)] string text);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetVehicleLicensePlate(int vehicle);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SetVehicleDamage(int vehicle, int index, float damage);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern float GetVehicleDamage(int vehicle, int index);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateVehicle(int model, double x, double y, double z, double heading);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateText3D([MarshalAs(UnmanagedType.LPStr)] string text, int size, double x,
            double y, double z, double rx, double ry, double rz);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetText3DAttached(int text3d, int attachType, int entity, double x, double y, double z,
            double rx, double ry, double rz, [MarshalAs(UnmanagedType.LPStr)] string socketName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetText3DVisibility(int text3d, int player, bool visible);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetText3DText(int text3d, [MarshalAs(UnmanagedType.LPStr)] string text);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPickupVisibility(int pickup, int player, bool visible);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetPickupScale(int pickup, ref double x, ref double y, ref double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPickupScale(int pickup, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreatePickup(int model, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetObjectModel(int obj);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectModel(int obj, int model);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectRotateAxis(int obj, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopObjectMove(int obj);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectMoveTo(int obj, double x, double y, double z, double speed);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsObjectMoving(int obj);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetObjectAttachmentInfo(int obj, ref int attachType, ref int entity);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsObjectAttached(int obj);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectAttached(int obj, int attachType, int entity, double x, double y, double z,
            double rx, double ry, double rz, [MarshalAs(UnmanagedType.LPStr)] string socketName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectDetached(int obj);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetObjectScale(int obj, ref double x, ref double y, ref double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectScale(int obj, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetObjectRotation(int obj, ref double x, ref double y, ref double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectRotation(int obj, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetObjectStreamDistance(int obj, double distance);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCFollowVehicle(int npc, int vehicle, double speed);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCFollowPlayer(int npc, int player, double speed);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCTargetLocation(int npc, double x, double y, double z, double speed);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCHeading(int npc, double heading);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetNPCHeading(int npc);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCAnimation(int npc, [MarshalAs(UnmanagedType.LPStr)] string animation, bool loop);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCHealth(int npc, double health);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetNPCHealth(int npc);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsStreamedIn([MarshalAs(UnmanagedType.LPStr)] string name, int player, int entity);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateNPC(double x, double y, double z, double heading);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetNPCRagdoll(int npc, bool enable);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetDoorModel(int door);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetDoorOpen(int door, bool open);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetDoorOpen(int door);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateDoor(int model, double x, double y, double z, double yaw, bool enableInteract);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DestroyEntity([MarshalAs(UnmanagedType.LPStr)] string entityName, int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetEntityDimension([MarshalAs(UnmanagedType.LPStr)] string entityName, int id,
            uint dim);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetEntityDimension([MarshalAs(UnmanagedType.LPStr)] string entityName, int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetNetworkStats(int source, ref int totalPacketLoss, ref int lastSecondPacketLoss,
            ref int messagesInResendBuffer,
            ref int bytesInResendBuffer, ref int bytesSend, ref int bytesReceived, ref int bytesResend,
            ref int totalBytesSend,
            ref int totalBytesReceived, ref bool isLimitedByCongestionControl,
            ref bool isLimitedByOutgoingBandwidthLimit);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsTimerValid(int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DestroyTimer(int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PauseTimer(int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UnpauseTimer(int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetTimerRemainingTime(int id);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateTimer([MarshalAs(UnmanagedType.LPStr)] string id, double interval);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Delay([MarshalAs(UnmanagedType.LPStr)] string name, long millis);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool CreateExplosion(int id, double x, double y, double z, uint dim, bool soundEnabled,
            double camShakeRadius, double radialForce, double damageRadius);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetMaxPlayers();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetServerName([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetServerName();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetServerTickRate();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetTheTickCount();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetTimeSeconds();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetDeltaSeconds();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetGameVersion();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetGameVersionAsString();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateObject(int model, double x, double y, double z, double rx, double ry,
            double rz, double sx, double sy, double sz);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetAllPackages();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsPackageStarted([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopPackage([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartPackage([MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetPlayerName(int player, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetPlayerName(int player);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SendPlayerChatMessage(int player, [MarshalAs(UnmanagedType.LPStr)] string message);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsEntityValid(int id, [MarshalAs(UnmanagedType.LPStr)] string name);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr[] GetKeysFromTable(IntPtr table);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void AddValueToTable(IntPtr table, IntPtr key, IntPtr val);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RemoveTableKey(IntPtr table, IntPtr key);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool ContainsTableKey(IntPtr table, IntPtr key);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetValueFromTable(IntPtr table, IntPtr key);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetLengthOfTable(IntPtr table);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr[] InvokePackage([MarshalAs(UnmanagedType.LPStr)] string importId, [MarshalAs(UnmanagedType.LPStr)] string funcName, IntPtr[] nVals, int len);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ImportPackage([MarshalAs(UnmanagedType.LPStr)] string packageName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetEntityPosition(int id, [MarshalAs(UnmanagedType.LPStr)] string name, double x, double y, double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetEntityPosition(int id, [MarshalAs(UnmanagedType.LPStr)] string name, ref double x, ref double y, ref double z);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ShutdownServer();
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterRemoteEvent([MarshalAs(UnmanagedType.LPStr)] string pluginId, [MarshalAs(UnmanagedType.LPStr)] string eventName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterCommand([MarshalAs(UnmanagedType.LPStr)] string pluginId, [MarshalAs(UnmanagedType.LPStr)] string commandName);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterCommandAlias([MarshalAs(UnmanagedType.LPStr)] string pluginId, [MarshalAs(UnmanagedType.LPStr)] string commandName, [MarshalAs(UnmanagedType.LPStr)] string alias);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void CallRemote(int player, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr[] nVals, int len);
        
        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_s", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue([MarshalAs(UnmanagedType.LPStr)] string val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_i", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue(int val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_d", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue(double val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_b", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue(bool val);

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_n", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValue();

        [DllImport(Bridge.DllName, EntryPoint = "CreateNValue_t", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateNValueTable();

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FreeNValue(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern NativeValue.Type GetNType(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetNDouble(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetNInt(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetNBoolean(IntPtr ptr);

        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetNString(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FreeVPtr(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetVDouble(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern float GetVFloat(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetVBool(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetVString(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ForceRuntimeRestart(bool complete);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ReleaseIntArray(IntPtr ptr);
        
        [DllImport(Bridge.DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetEntities([MarshalAs(UnmanagedType.LPStr)] string name, ref int len);
    }
}