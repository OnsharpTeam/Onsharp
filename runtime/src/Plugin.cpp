//
// Created by DasDarki on 25.06.2020.
//

#include <iostream>
#include <cstring>

#include <filesystem>
namespace fs = std::filesystem;

#ifdef _WIN32
#include <windows.h>
#endif

#include "Plugin.hpp"

#if defined _WIN32 || defined __CYGWIN__
#ifdef BUILDING_DLL
#ifdef __GNUC__
      #define EXPORTED extern "C" __attribute__ ((dllexport))
    #else
      #define EXPORTED extern "C" __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.
    #endif
#else
#ifdef __GNUC__
#define EXPORTED extern "C" __attribute__ ((dllimport))
#else
#define EXPORTED extern "C" __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.
#endif
#endif
#define NOT_EXPORTED
#else
#if __GNUC__ >= 4
    #define EXPORTED extern "C" __attribute__ ((visibility ("default")))
    #define NOT_EXPORTED extern "C"  __attribute__ ((visibility ("hidden")))
  #else
    #define EXPORTED
    #define NOT_EXPORTED
  #endif
#endif

#ifdef LUA_DEFINE
# undef LUA_DEFINE
#endif
#define LUA_DEFINE(name) Define(#name, [](lua_State *L) -> int

Lua::LuaArgs_t Plugin::CallLuaFunction(const char* LuaFunctionName, Lua::LuaArgs_t* Arguments) {
    Lua::LuaArgs_t ReturnValues;
    int ArgCount = lua_gettop(Plugin::MainScriptVM);
    lua_getglobal(Plugin::MainScriptVM, LuaFunctionName);
    int argc = 0;
    if (Arguments) {
        for (auto const& e : *Arguments) {
            Lua::PushValueToLua(e, Plugin::MainScriptVM);
            argc++;
        }
    }
    int Status = lua_pcall(Plugin::MainScriptVM, argc, LUA_MULTRET, 0);
    ArgCount = lua_gettop(Plugin::MainScriptVM) - ArgCount;
    if (Status == LUA_OK) {
        Lua::ParseArguments(Plugin::MainScriptVM, ReturnValues);
        lua_pop(Plugin::MainScriptVM, ArgCount);
    }
    return ReturnValues;
}

void Plugin::ClearLuaStack()
{
    Lua::LuaArgs_t stack;
    Lua::ParseArguments(Plugin::MainScriptVM, stack);
    int stack_size = static_cast<int>(stack.size());
    lua_pop(Plugin::MainScriptVM, stack_size);
}

Plugin::Plugin()
{
    LUA_DEFINE(CallBridge)
    {
        std::string key;
        Lua::LuaTable_t args_table;
        Lua::ParseArguments(L, key, args_table);
        int len = args_table->Count();
        void** args = new void*[len];
        args_table->ForEach([args](Lua::LuaValue k, Lua::LuaValue v) {
            (void) k;
            args[k.GetValue<int>()-1] = Plugin::Get()->CreateNValueByLua(std::move(v));
        });
        Plugin::Get()->ClearLuaStack();
        NValue* returnVal = Plugin::Get()->CallBridge(key.c_str(), args, len);
        if(key == "call-event") {
            Lua::LuaArgs_t argValues = Lua::BuildArgumentList(returnVal->GetLuaValue());
            return Lua::ReturnValues(L, argValues);
        }
        delete returnVal;
        return 0;
    });

    LUA_DEFINE(InitRuntimeEntries)
    {
        Plugin::Get()->GetBridge().InitRuntime();
        Plugin::Get()->InitDelegates();
        Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
        Lua::ReturnValues(L, argValues);
        return 0;
    });

    LUA_DEFINE(CallOnsharp)
    {
        std::string pluginId;
        std::string funcName;
        Lua::LuaTable_t args_table;
        Lua::ParseArguments(L, pluginId, funcName, args_table);
        int len = args_table->Count();
        void** args = new void*[len + 2];
        args[0] = Plugin::Get()->CreatNValueByString(pluginId);
        args[1] = Plugin::Get()->CreatNValueByString(funcName);
        args_table->ForEach([args](Lua::LuaValue k, Lua::LuaValue v) {
            (void) k;
            args[k.GetValue<int>()+1] = Plugin::Get()->CreateNValueByLua(std::move(v));
        });
        Plugin::Get()->ClearLuaStack();
        NValue* returnVal = Plugin::Get()->CallBridge("interop", args, len);
        Lua::LuaArgs_t argValues = Lua::BuildArgumentList(returnVal->GetLuaValue());
        return Lua::ReturnValues(L, argValues);
    });
}

//region Native Bridge Functions

EXPORTED Plugin::NValue* GetPropertyValue(const char* entityName, int entity, const char* propertyKey)
{
    std::string funcName = "Get" + std::string(entityName) + "PropertyValue";
    Lua::LuaArgs_t args = Lua::BuildArgumentList(entity, propertyKey);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED void SetPropertyValue(const char* entityName, int entity, const char* propertyKey, Plugin::NValue* propertyValue, bool sync)
{
    std::string funcName = "Set" + std::string(entityName) + "PropertyValue";
    Lua::LuaArgs_t args = Lua::BuildArgumentList(entity, propertyKey);
    propertyValue->AddAsArg(&args);
    args.emplace_back(sync);
    Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
}

EXPORTED bool SetPlayerRagdoll(int player, bool enable)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, enable);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetPlayerRagdoll", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED long long GetPlayerSteamId(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerSteamId", &args);
    return returnValues.at(0).GetValue<long long>();
}

EXPORTED float GetPlayerHeadSize(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerHeadSize", &args);
    return returnValues.at(0).GetValue<float>();
}

EXPORTED void SetPlayerHeadSize(int player, float size)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, size);
    Plugin::Get()->CallLuaFunction("SetPlayerHeadSize", &args);
}

EXPORTED void AttachPlayerParachute(int player, bool attach)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, attach);
    Plugin::Get()->CallLuaFunction("AttachPlayerParachute", &args);
}

EXPORTED void SetPlayerAnimation(int player, const char* animation)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, animation);
    Plugin::Get()->CallLuaFunction("SetPlayerAnimation", &args);
}

EXPORTED int GetPlayerGameVersion(int player)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerGameVersion", &argValues);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED Plugin::NValue* GetPlayerGUID(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerGUID", &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED Plugin::NValue* GetPlayerLocale(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerLocale", &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED void KickPlayer(int player, const char* reason)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, reason);
    Plugin::Get()->CallLuaFunction("KickPlayer", &args);
}

EXPORTED int GetPlayerPing(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerPing", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED Plugin::NValue* GetPlayerIP(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerIP", &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED long long GetPlayerRespawnTime(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerRespawnTime", &args);
    return returnValues.at(0).GetValue<long long>();
}

EXPORTED void SetPlayerRespawnTime(int player, long long msTime)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, msTime);
    Plugin::Get()->CallLuaFunction("SetPlayerRespawnTime", &args);
}

EXPORTED double GetPlayerArmor(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerArmor", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetPlayerArmor(int player, double armor)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, armor);
    Plugin::Get()->CallLuaFunction("SetPlayerArmor", &args);
}

EXPORTED double GetPlayerHealth(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerHealth", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetPlayerHealth(int player, double health)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, health);
    Plugin::Get()->CallLuaFunction("SetPlayerHealth", &args);
}

EXPORTED bool IsPlayerDead(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPlayerDead", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetPlayerSpectate(int player, bool spectate)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, spectate);
    Plugin::Get()->CallLuaFunction("SetPlayerSpectate", &args);
}

EXPORTED double GetPlayerHeading(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerHeading", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetPlayerHeading(int player, double heading)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, heading);
    Plugin::Get()->CallLuaFunction("SetPlayerHeading", &args);
}

EXPORTED bool EquipPlayerWeaponSlot(int player, int slot)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, slot);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("EquipPlayerWeaponSlot", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED int GetPlayerEquippedWeaponSlot(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerEquippedWeaponSlot", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void GetPlayerWeapon(int player, int slot, int* model, int* ammo)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, slot);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerWeapon", &args);
    *model = returnValues.at(0).GetValue<int>();
    *ammo = returnValues.at(1).GetValue<int>();
}

EXPORTED bool SetPlayerWeapon(int player, int weapon, int ammo, bool equip, int slot, bool loaded)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, weapon, ammo, equip, slot, loaded);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetPlayerWeapon", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED bool SetPlayerWeaponStat(int player, int weapon, const char* stat, double value)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, weapon, stat, value);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetPlayerWeaponStat", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void RemovePlayerFromVehicle(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Plugin::Get()->CallLuaFunction("RemovePlayerFromVehicle", &args);
}

EXPORTED void SetPlayerInVehicle(int player, int vehicle, int seat)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, vehicle, seat);
    Plugin::Get()->CallLuaFunction("SetPlayerInVehicle", &args);
}

EXPORTED int GetPlayerVehicleSeat(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerVehicleSeat", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED int GetPlayerVehicle(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerVehicle", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED bool IsPlayerReloading(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPlayerReloading", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED bool IsPlayerAiming(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPlayerAiming", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED double GetPlayerMovementSpeed(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerMovementSpeed", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED int GetPlayerMovementMode(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerMovementMode", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED int GetPlayerState(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerState", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED bool SetPlayerVoiceDimension(int player, unsigned int dim)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, dim);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetPlayerVoiceDimension", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED bool IsPlayerTalking(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPlayerTalking", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED bool IsPlayerVoiceEnabled(int player)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPlayerVoiceEnabled", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetPlayerVoiceEnabled(int player, bool enable)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, enable);
    Plugin::Get()->CallLuaFunction("SetPlayerVoiceEnabled", &args);
}

EXPORTED bool IsPlayerVoiceChannel(int player, int channel)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, channel);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPlayerVoiceChannel", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetPlayerVoiceChannel(int player, int channel, bool enable)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, channel, enable);
    Plugin::Get()->CallLuaFunction("SetPlayerSpawnLocation", &args);
}

EXPORTED void SetPlayerSpawnLocation(int player, double x, double y, double z, double heading)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, x, y, z, heading);
    Plugin::Get()->CallLuaFunction("SetPlayerSpawnLocation", &args);
}

EXPORTED void EnableVehicleBackfire(int vehicle, bool enable)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, enable);
    Plugin::Get()->CallLuaFunction("EnableVehicleBackfire", &args);
}

EXPORTED void AttachVehicleNitro(int vehicle, bool attach)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, attach);
    Plugin::Get()->CallLuaFunction("AttachVehicleNitro", &args);
}

EXPORTED bool SetVehicleDamage(int vehicle, int index, float damage)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, index, damage);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetVehicleDamage", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED bool GetVehicleLightEnabled(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleLightEnabled", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetVehicleLightEnabled(int vehicle, bool enabled)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, enabled);
    Plugin::Get()->CallLuaFunction("SetVehicleLightEnabled", &args);
}

EXPORTED bool GetVehicleEngineState(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleEngineState", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void StopVehicleEngine(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Plugin::Get()->CallLuaFunction("StopVehicleEngine", &args);
}

EXPORTED void StartVehicleEngine(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Plugin::Get()->CallLuaFunction("StartVehicleEngine", &args);
}

EXPORTED void SetVehicleTrunkRatio(int vehicle, double ratio)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, ratio);
    Plugin::Get()->CallLuaFunction("SetVehicleTrunkRatio", &args);
}

EXPORTED double GetVehicleTrunkRatio(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleTrunkRatio", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetVehicleHoodRatio(int vehicle, double ratio)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, ratio);
    Plugin::Get()->CallLuaFunction("SetVehicleHoodRatio", &args);
}

EXPORTED double GetVehicleHoodRatio(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleHoodRatio", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED int GetVehicleGear(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleGear", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetVehicleAngularVelocity(int vehicle, double x, double y, double z, bool reset)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, x, y, z, reset);
    Plugin::Get()->CallLuaFunction("SetVehicleAngularVelocity", &args);
}

EXPORTED void SetVehicleLinearVelocity(int vehicle, double x, double y, double z, bool reset)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, x, y, z, reset);
    Plugin::Get()->CallLuaFunction("SetVehicleLinearVelocity", &args);
}

EXPORTED Plugin::NValue* GetVehicleColor(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleColor", &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED void SetVehicleColor(int vehicle, const char* hexColor)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, hexColor);
    Plugin::Get()->CallLuaFunction("SetVehicleColor", &args);
}

EXPORTED int GetVehicleNumberOfSeats(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleNumberOfSeats", &args);
    Lua::LuaValue val = returnValues.at(0);
    if(val.IsBoolean()) return 0;
    return val.GetValue<int>();
}

EXPORTED int GetVehiclePassenger(int vehicle, int seat)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, seat);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehiclePassenger", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED int GetVehicleDriver(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleDriver", &args);
    Lua::LuaValue val = returnValues.at(0);
    if(val.IsBoolean()) return 0;
    return val.GetValue<int>();
}

EXPORTED void GetVehicleVelocity(int vehicle, double* x, double* y, double* z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleVelocity", &args);
    *x = returnValues.at(0).GetValue<double>();
    *y = returnValues.at(1).GetValue<double>();
    *z = returnValues.at(2).GetValue<double>();
}

EXPORTED double GetVehicleHealth(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleHealth", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetVehicleHealth(int vehicle, double health)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, health);
    Plugin::Get()->CallLuaFunction("SetVehicleHealth", &args);
}

EXPORTED double GetVehicleHeading(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleHeading", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetVehicleHeading(int vehicle, double heading)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, heading);
    Plugin::Get()->CallLuaFunction("SetVehicleHeading", &args);
}

EXPORTED void GetVehicleRotation(int vehicle, double* x, double* y, double* z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleRotation", &args);
    *x = returnValues.at(0).GetValue<double>();
    *y = returnValues.at(1).GetValue<double>();
    *z = returnValues.at(2).GetValue<double>();
}

EXPORTED void SetVehicleRotation(int vehicle, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, x, y, z);
    Plugin::Get()->CallLuaFunction("SetVehicleRotation", &args);
}

EXPORTED bool SetVehicleRespawnParams(int vehicle, bool enableRespawn, long long respawnTime, bool repairOnRespawn)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, enableRespawn, respawnTime, repairOnRespawn);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetVehicleRespawnParams", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED Plugin::NValue* GetVehicleModelName(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleModelName", &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED int GetVehicleModel(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleModel", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetVehicleLicensePlate(int vehicle, const char* text)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, text);
    Plugin::Get()->CallLuaFunction("SetVehicleLicensePlate", &args);
}

EXPORTED Plugin::NValue* GetVehicleLicensePlate(int vehicle)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleLicensePlate", &args);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED float GetVehicleDamage(int vehicle, int index)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(vehicle, index);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetVehicleDamage", &args);
    return returnValues.at(0).GetValue<float>();
}

EXPORTED int CreateVehicle(int model, double x, double y, double z, double heading)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(model, x, y, z, heading);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreateVehicle", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetText3DText(int text3d, const char* text)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(text3d, text);
    Plugin::Get()->CallLuaFunction("SetText3DText", &args);
}

EXPORTED void SetText3DVisibility(int text3d, int player, bool visible)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(text3d, player, visible);
    Plugin::Get()->CallLuaFunction("SetText3DVisibility", &args);
}

EXPORTED void SetText3DAttached(int text3d, int attachType, int entity, double x, double y, double z,
                                double rx, double ry, double rz, const char* socketName)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(text3d, attachType, entity, x, y, z, rx, ry, rz, socketName);
    Plugin::Get()->CallLuaFunction("SetText3DAttached", &args);
}

EXPORTED int CreateText3D(const char* text, int size, double x, double y, double z, double rx, double ry, double rz)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(text, size, x, y, z, rx, ry, rz);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreateText3D", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetPickupVisibility(int pickup, int player, bool visible)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(pickup, player, visible);
    Plugin::Get()->CallLuaFunction("SetPickupVisibility", &args);
}

EXPORTED void GetPickupScale(int pickup, double* x, double* y, double* z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(pickup);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPickupScale", &args);
    *x = returnValues.at(0).GetValue<double>();
    *y = returnValues.at(1).GetValue<double>();
    *z = returnValues.at(2).GetValue<double>();
}

EXPORTED void SetPickupScale(int pickup, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(pickup, x, y, z);
    Plugin::Get()->CallLuaFunction("SetPickupScale", &args);
}

EXPORTED int CreatePickup(int model, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(model, x, y, z);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreatePickup", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED int GetObjectModel(int obj)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetObjectModel", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetObjectModel(int obj, int model)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, model);
    Plugin::Get()->CallLuaFunction("SetObjectModel", &args);
}

EXPORTED void SetObjectRotateAxis(int obj, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, x, y, z);
    Plugin::Get()->CallLuaFunction("SetObjectRotateAxis", &args);
}

EXPORTED void StopObjectMove(int obj)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Plugin::Get()->CallLuaFunction("StopObjectMove", &args);
}

EXPORTED void SetObjectMoveTo(int obj, double x, double y, double z, double speed)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, x, y, z, speed);
    Plugin::Get()->CallLuaFunction("SetObjectMoveTo", &args);
}

EXPORTED bool IsObjectMoving(int obj)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsObjectMoving", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void GetObjectAttachmentInfo(int obj, int* attachType, int* entity)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetObjectAttachmentInfo", &args);
    *attachType = returnValues.at(0).GetValue<int>();
    *entity = returnValues.at(0).GetValue<int>();
}

EXPORTED bool IsObjectAttached(int obj)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsObjectAttached", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetObjectDetached(int obj)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Plugin::Get()->CallLuaFunction("SetObjectDetached", &args);
}

EXPORTED void SetObjectAttached(int obj, int attachType, int entity, double x, double y, double z,
                                double rx, double ry, double rz, const char* socketName)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, attachType, entity, x, y, z, rx, ry, rz, socketName);
    Plugin::Get()->CallLuaFunction("SetObjectAttached", &args);
}

EXPORTED void GetObjectScale(int obj, double* x, double* y, double* z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetObjectScale", &args);
    *x = returnValues.at(0).GetValue<double>();
    *y = returnValues.at(1).GetValue<double>();
    *z = returnValues.at(2).GetValue<double>();
}

EXPORTED void SetObjectScale(int obj, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, x, y, z);
    Plugin::Get()->CallLuaFunction("SetObjectScale", &args);
}

EXPORTED void GetObjectRotation(int obj, double* x, double* y, double* z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("SetObjectRotation", &args);
    *x = returnValues.at(0).GetValue<double>();
    *y = returnValues.at(1).GetValue<double>();
    *z = returnValues.at(2).GetValue<double>();
}

EXPORTED void SetObjectRotation(int obj, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, x, y, z);
    Plugin::Get()->CallLuaFunction("SetObjectRotation", &args);
}

EXPORTED void SetObjectStreamDistance(int obj, double distance)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(obj, distance);
    Plugin::Get()->CallLuaFunction("SetObjectStreamDistance", &args);
}

EXPORTED int CreateObject(int model, double x, double y, double z, double rx, double ry, double rz, double sx, double sy, double sz)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(model, x, y, z, rx, ry, rz, sx, sy, sz);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreateObject", &argValues);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetNPCFollowVehicle(int npc, int vehicle, double speed)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, vehicle, speed);
    Plugin::Get()->CallLuaFunction("SetNPCFollowVehicle", &args);
}

EXPORTED void SetNPCFollowPlayer(int npc, int player, double speed)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, player, speed);
    Plugin::Get()->CallLuaFunction("SetNPCFollowPlayer", &args);
}

EXPORTED void SetNPCTargetLocation(int npc, double x, double y, double z, double speed)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, x, y, z, speed);
    Plugin::Get()->CallLuaFunction("SetNPCTargetLocation", &args);
}

EXPORTED double GetNPCHeading(int npc)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetNPCHeading", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetNPCHeading(int npc, double heading)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, heading);
    Plugin::Get()->CallLuaFunction("SetNPCHeading", &args);
}

EXPORTED void SetNPCAnimation(int npc, const char* animation, bool loop)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, animation, loop);
    Plugin::Get()->CallLuaFunction("SetNPCAnimation", &args);
}

EXPORTED double GetNPCHealth(int npc)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetNPCHealth", &args);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED void SetNPCHealth(int npc, double health)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, health);
    Plugin::Get()->CallLuaFunction("SetNPCHealth", &args);
}

EXPORTED bool IsStreamedIn(const char* name, int player, int entity)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(player, entity);
    std::string funcName = "Is" + std::string(name) + "StreamedIn";
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED int CreateNPC(double x, double y, double z, double heading)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(x, y, z, heading);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreateNPC", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void SetNPCRagdoll(int npc, bool enable)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(npc, enable);
    Plugin::Get()->CallLuaFunction("SetNPCRagdoll", &args);
}

EXPORTED int GetDoorModel(int door)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(door);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetDoorModel", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED bool GetDoorOpen(int door)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(door);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsDoorOpen", &args);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetDoorOpen(int door, bool open)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(door, open);
    Plugin::Get()->CallLuaFunction("SetDoorOpen", &args);
}

EXPORTED int CreateDoor(int model, double x, double y, double z, double yaw, bool enableInteract)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(model, x, y, z, yaw, enableInteract);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreateDoor", &args);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED unsigned int GetEntityDimension(const char* entityName, int id)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(id);
    std::string funcName = "Get" + std::string(entityName) + "Dimension";
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
    return returnValues.at(0).GetValue<unsigned int>();
}

EXPORTED void SetEntityDimension(const char* entityName, int id, unsigned int dim)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(id, dim);
    std::string funcName = "Set" + std::string(entityName) + "Dimension";
    Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
}

EXPORTED void DestroyEntity(const char* entityName, int id)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(id);
    std::string funcName = "Destroy" + std::string(entityName);
    Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
}

EXPORTED void GetNetworkStats(int source, int* totalPacketLoss, int* lastSecondPacketLoss, int* messagesInResendBuffer,
        int* bytesInResendBuffer, int* bytesSend, int* bytesReceived, int* bytesResend, int* totalBytesSend,
        int* totalBytesReceived, bool* isLimitedByCongestionControl, bool* isLimitedByOutgoingBandwidthLimit) {
    Lua::LuaArgs_t argValues = source <= 0 ? Lua::BuildArgumentList() : Lua::BuildArgumentList(source);
    std::string funcName = "Get" + std::string((source <= 0 ? "" : "Player")) + "NetworkStats";
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(funcName.c_str(), &argValues);
    *totalPacketLoss = returnValues.at(0).GetValue<int>();
    *lastSecondPacketLoss = returnValues.at(1).GetValue<int>();
    *messagesInResendBuffer = returnValues.at(2).GetValue<int>();
    *bytesInResendBuffer = returnValues.at(3).GetValue<int>();
    *bytesSend = returnValues.at(4).GetValue<int>();
    *bytesReceived = returnValues.at(5).GetValue<int>();
    *bytesResend = returnValues.at(6).GetValue<int>();
    *totalBytesSend = returnValues.at(7).GetValue<int>();
    *totalBytesReceived = returnValues.at(8).GetValue<int>();
    *isLimitedByCongestionControl = returnValues.at(9).GetValue<bool>();
    *isLimitedByOutgoingBandwidthLimit = returnValues.at(10).GetValue<bool>();
}

EXPORTED void DestroyTimer(int id)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Plugin::Get()->CallLuaFunction("DestroyTimer", &argValues);
}

EXPORTED void PauseTimer(int id)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Plugin::Get()->CallLuaFunction("PauseTimer", &argValues);
}

EXPORTED void UnpauseTimer(int id)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Plugin::Get()->CallLuaFunction("UnpauseTimer", &argValues);
}

EXPORTED double GetTimerRemainingTime(int id)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetTimerRemainingTime", &argValues);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED bool IsTimerValid(int id)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsValidTimer", &argValues);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED int CreateTimer(const char* id, double interval)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id, interval);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("Onsharp_CreateTimer", &argValues);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED void Delay(const char* id, long long millis)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id, millis);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("Onsharp_Delay", &argValues);
}

EXPORTED bool CreateExplosion(int id, double x, double y, double z, unsigned int dim, bool soundEnabled,
                              double camShakeRadius, double radialForce, double damageRadius)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id, x, y, z, dim, soundEnabled, camShakeRadius, radialForce, damageRadius);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetMaxPlayers", &argValues);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void SetServerName(const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(name);
    Plugin::Get()->CallLuaFunction("SetServerName", &argValues);
}

EXPORTED Plugin::NValue* GetServerName()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetServerName", &argValues);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED int GetMaxPlayers()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetMaxPlayers", &argValues);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED double GetServerTickRate()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetServerTickRate", &argValues);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED double GetTheTickCount()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetTickCount", &argValues);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED double GetTimeSeconds()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetTimeSeconds", &argValues);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED double GetDeltaSeconds()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetDeltaSeconds", &argValues);
    return returnValues.at(0).GetValue<double>();
}

EXPORTED Plugin::NValue* GetGameVersionAsString()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetGameVersionString", &argValues);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED int GetGameVersion()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetGameVersion", &argValues);
    return returnValues.at(0).GetValue<int>();
}

EXPORTED Plugin::NValue* GetAllPackages()
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetAllPackages", &argValues);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED bool IsPackageStarted(const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(name);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("IsPackageStarted", &argValues);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void StopPackage(const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(name);
    Plugin::Get()->CallLuaFunction("StopPackage", &argValues);
}

EXPORTED void StartPackage(const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(name);
    Plugin::Get()->CallLuaFunction("StartPackage", &argValues);
}

EXPORTED void SetPlayerName(int player, const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player, name);
    Plugin::Get()->CallLuaFunction("SetPlayerName", &argValues);
}

EXPORTED Plugin::NValue* GetPlayerName(int player)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerName", &argValues);
    return Plugin::Get()->CreateNValueByLua(returnValues.at(0));
}

EXPORTED void SendPlayerChatMessage(int player, const char* message)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player, message);
    Plugin::Get()->CallLuaFunction("AddPlayerChat", &argValues);
}

EXPORTED Plugin::NValue** GetKeysFromTable(Plugin::NValue* table)
{
    auto rVals = new Plugin::NValue*[table->tVal->Count()];
    int idx = 0;
    table->tVal->ForEach([rVals, &idx](Lua::LuaValue k, Lua::LuaValue v) {
        (void) v;
        rVals[idx] = Plugin::Get()->CreateNValueByLua(std::move(k));
        idx++;
    });
    return rVals;
}

EXPORTED void AddValueToTable(Plugin::NValue* table, Plugin::NValue* key, Plugin::NValue* val)
{
    table->tVal->Add(key->GetLuaValue(), val->GetLuaValue());
}

EXPORTED void RemoveTableKey(Plugin::NValue* table, Plugin::NValue* key)
{
    table->tVal->Remove(key->GetLuaValue());
}

EXPORTED bool ContainsTableKey(Plugin::NValue* table, Plugin::NValue* key)
{
    return table->tVal->Exists(key->GetLuaValue());
}

EXPORTED Plugin::NValue* GetValueFromTable(Plugin::NValue* table, Plugin::NValue* key)
{
    bool _break = false;
    auto currVal = new Lua::LuaValue;
    Lua::LuaValue k2 = key->GetLuaValue();
    table->tVal->ForEach([&_break, &currVal, k2](Lua::LuaValue k, Lua::LuaValue v) {
        if(_break) return;
        if(k == k2) {
            _break = true;
            *currVal = std::move(v);
        }
    });
    return Plugin::Get()->CreateNValueByLua(currVal);
}

EXPORTED int GetLengthOfTable(Plugin::NValue* table)
{
    return table->tVal->Count();
}

EXPORTED Plugin::NValue** InvokePackage(const char* importId, const char* funcName, Plugin::NValue* nVals[], int len)
{
    Lua::LuaArgs_t arg_list = Lua::BuildArgumentList(importId, funcName);
    for(int i = 0; i < len; i++)
    {
        nVals[i]->AddAsArg(&arg_list);
    }

    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("Onsharp_InvokePackage", &arg_list);
    auto rVals = new Plugin::NValue*[returnValues.size()];
    for(int i = 0; i < static_cast<int>(returnValues.size()); i++)
    {
        rVals[i] = Plugin::Get()->CreateNValueByLua(returnValues.at(i));
    }

    return rVals;
}

EXPORTED void ImportPackage(const char* packageName)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(packageName);
    Plugin::Get()->CallLuaFunction("Onsharp_ImportPackage", &args);
}

EXPORTED void GetEntityPosition(int id, const char* entityName, double* x, double* y, double* z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(id);
    std::string funcName = "Get" + std::string(entityName) + "Location";
    Lua::LuaArgs_t returnVals = Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
    *x = returnVals.at(0).GetValue<double>();
    *y = returnVals.at(1).GetValue<double>();
    *z = returnVals.at(2).GetValue<double>();
}

EXPORTED void SetEntityPosition(int id, const char* entityName, double x, double y, double z)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(id, x, y, z);
    std::string funcName = "Set" + std::string(entityName) + "Location";
    Plugin::Get()->CallLuaFunction(funcName.c_str(), &args);
}

EXPORTED void ShutdownServer()
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList();
    Plugin::Get()->CallLuaFunction("ServerExit", &args);
}

EXPORTED void RegisterRemoteEvent(const char* pluginId, const char* eventName)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(pluginId, eventName);
    Plugin::Get()->CallLuaFunction("Onsharp_RegisterRemoteEvent", &args);
}

EXPORTED void RegisterCommand(const char* pluginId, const char* commandName)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(pluginId, commandName);
    Plugin::Get()->CallLuaFunction("Onsharp_RegisterCommand", &args);
}

EXPORTED void RegisterCommandAlias(const char* pluginId, const char* commandName, const char* alias)
{
    Lua::LuaArgs_t args = Lua::BuildArgumentList(pluginId, commandName, alias);
    Plugin::Get()->CallLuaFunction("Onsharp_RegisterCommandAlias", &args);
}

EXPORTED Plugin::NValue* CreateNValue_s(const char* val)
{
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::STRING;
    nVal->sVal = std::string(val);
    return nVal;
}

EXPORTED Plugin::NValue* CreateNValue_i(int val)
{
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::INTEGER;
    nVal->iVal = val;
    return nVal;
}

EXPORTED Plugin::NValue* CreateNValue_d(double val)
{
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::DOUBLE;
    nVal->dVal = val;
    return nVal;
}

EXPORTED Plugin::NValue* CreateNValue_b(bool val)
{
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::BOOLEAN;
    nVal->bVal = val;
    return nVal;
}

EXPORTED Plugin::NValue* CreateNValue_t()
{
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::TABLE;
    return nVal;
}

EXPORTED Plugin::NValue* CreateNValue_n()
{
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::NONE;
    return nVal;
}

EXPORTED double GetNDouble(Plugin::NValue* nPtr)
{
    return nPtr->dVal;
}

EXPORTED int GetNInt(Plugin::NValue* nPtr)
{
    return nPtr->iVal;
}

EXPORTED const char* GetNString(Plugin::NValue* nPtr)
{
    return nPtr->sVal.c_str();
}

EXPORTED bool GetNBoolean(Plugin::NValue* nPtr)
{
    return nPtr->bVal;
}

EXPORTED void FreeNValue(Plugin::NValue* nPtr)
{
    delete nPtr;
}

EXPORTED Plugin::NTYPE GetNType(Plugin::NValue* nPtr)
{
    return nPtr->type;
}

EXPORTED int* GetEntities(const char* entityName, int* len)
{
    std::string sFuncName = "GetAll" + std::string(entityName);
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(sFuncName.c_str(), &argValues);
    auto entityTable = returnValues.at(0).GetValue<Lua::LuaTable_t>();
    *len = entityTable->Count();
    int* entities = new int[entityTable->Count()];
    int idx = 0;
    entityTable->ForEach([entities, &idx](Lua::LuaValue k, Lua::LuaValue v) {
        (void) k;
        entities[idx] = (int)v.GetValue<float>();
        idx++;
    });
    return entities;
}

EXPORTED int ReleaseIntArray(int* lArray)
{
    delete[] lArray;
    return 0;
}

EXPORTED void ForceRuntimeRestart(bool complete)
{
    (void) complete;
    Plugin::Get()->GetBridge().Restart();
}

EXPORTED bool IsEntityValid(int id, const char* entityName)
{
    std::string sFuncName = "IsValid" + std::string(entityName);
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(sFuncName.c_str(), &argValues);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void CallRemote(int player, const char* name, Plugin::NValue* nVals[], int len)
{
    Lua::LuaArgs_t arg_list = Lua::BuildArgumentList(player, name);
    for(int i = 0; i < len; i++)
    {
        nVals[i]->AddAsArg(&arg_list);
    }

    Plugin::Get()->CallLuaFunction("CallRemoteEvent", &arg_list);
}

//endregion