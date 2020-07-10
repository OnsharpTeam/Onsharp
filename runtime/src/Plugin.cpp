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

#ifdef _WIN32
# define EXPORTED extern "C" __declspec(dllexport)
#else
# define EXPORTED
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
        int idx = 0;
        args_table->ForEach([args, &idx](Lua::LuaValue k, Lua::LuaValue v) {
            args[idx] = Plugin::Get()->CreateNValueByLua(std::move(v));
            idx++;
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
        int idx = 2;
        args_table->ForEach([args, &idx](Lua::LuaValue k, Lua::LuaValue v) {
            args[idx] = Plugin::Get()->CreateNValueByLua(std::move(v));
            idx++;
        });
        Plugin::Get()->ClearLuaStack();
        NValue* returnVal = Plugin::Get()->CallBridge("interop", args, len);
        Lua::LuaArgs_t argValues = Lua::BuildArgumentList(returnVal->GetLuaValue());
        return Lua::ReturnValues(L, argValues);
    });
}

//region Native Bridge Functions

EXPORTED void GetNetworkStats(int* totalPacketLoss, int* lastSecondPacketLoss, int* messagesInResendBuffer,
        int* bytesInResendBuffer, int* bytesSend, int* bytesReceived, int* bytesResend, int* totalBytesSend,
        int* totalBytesReceived, bool* isLimitedByCongestionControl, bool* isLimitedByOutgoingBandwidthLimit) {
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetNetworkStats", &argValues);
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

EXPORTED void Delay(const char* id, long millis)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id, millis);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("Onsharp_Delay", &argValues);
}

EXPORTED bool CreateExplosion(byte id, double x, double y, double z, unsigned int dim, bool soundEnabled,
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

EXPORTED int CreateObject(int model, double x, double y, double z, double rx, double ry, double rz, double sx, double sy, double sz)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(model, x, y, z, rx, ry, rz, sx, sy, sz);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("CreateObject", &argValues);
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
    for(int i = 0; i < returnValues.size(); i++)
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
    (void*) complete;
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