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

//endregion