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
}

//region Native Bridge Functions

EXPORTED Plugin::NValue* CreateNValue_s(const char* val)
{
    char* cVal = new char;
    strcpy(cVal, val);
    auto nVal = new Plugin::NValue;
    nVal->type = Plugin::NTYPE::STRING;
    nVal->sVal = cVal;
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
    return nPtr->sVal;
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

EXPORTED void SetPlayerName(long player, const char* name)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player, name);
    Plugin::Get()->CallLuaFunction("SetPlayerName", &argValues);
}

EXPORTED const char* GetPlayerName(long player)
{
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(player);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction("GetPlayerName", &argValues);
    auto name = returnValues.at(0).GetValue<std::string>();
    const char* namePtr = name.c_str();
    return namePtr;
}

EXPORTED void SendPlayerChatMessage(long player, const char* message)
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

EXPORTED bool IsEntityValid(long id, const char* entityName)
{
    std::string sFuncName = "IsValid" + std::string(entityName);
    Lua::LuaArgs_t argValues = Lua::BuildArgumentList(id);
    Lua::LuaArgs_t returnValues = Plugin::Get()->CallLuaFunction(sFuncName.c_str(), &argValues);
    return returnValues.at(0).GetValue<bool>();
}

EXPORTED void CallRemote(long player, const char* name, Plugin::NValue* nVals[], int len)
{
    Lua::LuaArgs_t arg_list;
    arg_list.push_back(new Lua::LuaValue(player));
    arg_list.push_back(new Lua::LuaValue(name));
    for(int i = 0; i < len; i++)
    {
        arg_list.push_back(nVals[i]->GetLuaValue());
    }

    Plugin::Get()->CallLuaFunction("CallRemoteEvent", &arg_list);
}

//endregion