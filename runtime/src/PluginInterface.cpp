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
}

EXPORT(void) OnPackageLoad(const char *PackageName, lua_State *L)
{
    auto pn = new std::string(PackageName);
    if (*pn == "onsharp") {
        Plugin::Get()->Setup(L);
        for (auto const &f : Plugin::Get()->GetFunctions())
            Lua::RegisterPluginFunction(L, std::get<0>(f), std::get<1>(f));
    }
}

EXPORT(void) OnPackageUnload(const char *PackageName)
{
    auto pn = new std::string(PackageName);
    if (*pn == "onsharp") {
        Plugin::Get()->GetBridge().Stop();
    }
}
