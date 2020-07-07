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
        NValue* returnVal = Plugin::Get()->CallBridge(key.c_str(), args, len);
        Lua::LuaArgs_t argValues = Lua::BuildArgumentList(returnVal->GetLuaValue());
        Lua::ReturnValues(L, argValues);
        return 1;
    });

    LUA_DEFINE(InitRuntimeEntries)
    {
        Plugin::Get()->GetBridge().InitRuntime();
        Plugin::Get()->InitDelegates();
        Lua::LuaArgs_t argValues = Lua::BuildArgumentList();
        Lua::ReturnValues(L, argValues);
        return 1;
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
        NValue* returnVal = Plugin::Get()->CallBridge("interop", args, len);
        Lua::LuaArgs_t argValues = Lua::BuildArgumentList(returnVal->GetLuaValue());
        Lua::ReturnValues(L, argValues);
        return 1;
    });
}

//region Native Bridge Functions

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

EXPORTED void SetPlayerName(int player, const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player, name);
    Plugin::Get()->CallLuaFunction("SetPlayerName", &argValues);
}

EXPORTED Plugin::NValue* GetPlayerName(int player)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerName", &argValues);
    auto name = returnValues.at(0).GetValue<std::string>();
    Plugin::NValue* nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::STRING;
    nVal->sVal = name;
    return nVal;
}

EXPORTED void SendPlayerChatMessage(int player, const char* message)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player, message);
    Plugin::Get()->CallLuaFunction("AddPlayerChat", &argValues);
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