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

Lua::LuaArgs_t CallLuaFunction(lua_State* ScriptVM, const char* LuaFunctionName, Lua::LuaArgs_t* Arguments) {
    Lua::LuaArgs_t ReturnValues;
    int ArgCount = lua_gettop(ScriptVM);
    lua_getglobal(ScriptVM, LuaFunctionName);
    int argc = 0;
    if (Arguments) {
        for (auto const& e : *Arguments) {
            Lua::PushValueToLua(e, ScriptVM);
            argc++;
        }
    }
    int Status = lua_pcall(ScriptVM, argc, LUA_MULTRET, 0);
    ArgCount = lua_gettop(ScriptVM) - ArgCount;
    if (Status == LUA_OK) {
        Lua::ParseArguments(ScriptVM, ReturnValues);
        lua_pop(ScriptVM, ArgCount);
    }
    return ReturnValues;
}

Plugin::Plugin()
{
}