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
    Plugin::Get();
    Onset::Plugin::Get()->Log("OnsharpRuntime (" PLUGIN_VERSION ") loaded!");
    return PLUGIN_API_VERSION;
}

EXPORT(void) OnPluginStop()
{
    Onset::Plugin::Get()->Log("!");
    Plugin::Get()->GetBridge().stop();
    Plugin::Singleton::Destroy();
    Onset::Plugin::Get()->Log("OnsharpRuntime unloaded!");
    Onset::Plugin::Destroy();
}

EXPORT(void) OnPluginTick(float DeltaSeconds)
{
    (void)DeltaSeconds;
}

EXPORT(void) OnPackageLoad(const char *PackageName, lua_State *L)
{
    (void)PackageName;
    for (auto const &f : Plugin::Get()->GetFunctions())
        Lua::RegisterPluginFunction(L, std::get<0>(f), std::get<1>(f));
    Plugin::Get()->AddPackage(PackageName, L);
}

EXPORT(void) OnPackageUnload(const char *PackageName)
{
    Plugin::Get()->RemovePackage(PackageName);
}
