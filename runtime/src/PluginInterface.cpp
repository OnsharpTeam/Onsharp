//
// Created by DasDarki on 25.06.2020.
//
#include <PluginSDK.h>
#include "Plugin.hpp"
#include "version.hpp"

Onset::IServerPlugin* Onset::Plugin::_instance = nullptr;

EXPORT(int) OnPluginGetApiVersion()
{
    return PLUGIN_API_VERSION;
}

EXPORT(void) OnPluginCreateInterface(Onset::IBaseInterface *PluginInterface)
{
    Onset::Plugin::Init(PluginInterface);
}

EXPORT(int) OnPluginStart()
{
    Onset::Plugin::Get()->Log("OnsharpRuntime (" PLUGIN_VERSION ") loaded!");
    return PLUGIN_API_VERSION;
}

EXPORT(void) OnPluginStop()
{
    Plugin::Singleton::Destroy();
    Onset::Plugin::Destroy();
}

EXPORT(void) OnPluginTick(float DeltaSeconds)
{
    (void)DeltaSeconds;
    Plugin::Get()->GetBridge().TriggerTick();
}

EXPORT(void) OnPackageLoad(const char *PackageName, lua_State *L)
{
    auto pn = new std::string(PackageName);
    if (*pn == "onsharp") {
        for (auto const &f : Plugin::Get()->GetFunctions()){
            const char* funcName = std::get<0>(f);
            auto stFuncName = new std::string(funcName);
            if(*stFuncName == "CallOnsharp") continue;
            Lua::RegisterPluginFunction(L, funcName, std::get<1>(f));
        }
        Plugin::Get()->Setup(L);
    }else{
        for (auto const &f : Plugin::Get()->GetFunctions()){
            const char* funcName = std::get<0>(f);
            auto stFuncName = new std::string(funcName);
            if(*stFuncName != "CallOnsharp") continue;
            Lua::RegisterPluginFunction(L, funcName, std::get<1>(f));
            break;
        }
    }
}

EXPORT(void) OnPackageUnload(const char *PackageName)
{
    auto pn = new std::string(PackageName);
    if (*pn == "onsharp") {
        Plugin::Get()->GetBridge().Stop();
    }
}
