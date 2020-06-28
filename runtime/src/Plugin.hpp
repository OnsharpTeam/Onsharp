#pragma once

#include <vector>
#include <tuple>
#include <map>
#include <functional>
#include <PluginSDK.h>
#include "Singleton.hpp"
#include "NetBridge.hpp"

class Plugin : public Singleton<Plugin>
{
    friend class Singleton<Plugin>;
private:
    Plugin();
    ~Plugin() = default;
    std::map<std::string, lua_State*> packageStates;
    std::map<lua_State*, std::string> statePackages;
    NetBridge bridge;
    lua_State* MainScriptVM;

private:
    using FuncInfo_t = std::tuple<const char *, lua_CFunction>;
    std::vector<FuncInfo_t> _func_list;

private:
    inline void Define(const char * name, lua_CFunction func)
    {
        _func_list.emplace_back(name, func);
    }

public:
    decltype(_func_list) const &GetFunctions() const
    {
        return _func_list;
    }
    void AddPackage(std::string name, lua_State* state) {
        this->packageStates[name] = state;
        this->statePackages[state] = name;
    }
    void RemovePackage(std::string name) {
        this->statePackages[this->packageStates[name]] = nullptr;
        this->packageStates[name] = nullptr;
    }
    lua_State* GetPackageState(std::string name) {
        return this->packageStates[name];
    }
    std::string GetStatePackage(lua_State* L) {
        return this->statePackages[L];
    }
    void Setup(lua_State* L) {
        this->MainScriptVM = L;
    }
    NetBridge GetBridge() {
        return this->bridge;
    }
    Lua::LuaArgs_t CallLuaFunction(const char* LuaFunctionName, Lua::LuaArgs_t* Arguments);
};