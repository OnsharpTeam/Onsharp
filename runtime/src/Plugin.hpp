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
    bool isSetup = false;
    enum class NTYPE
    {
        NONE = 0,
        STRING = 1,
        DOUBLE = 2,
        INTEGER = 3,
        BOOLEAN = 4
    };

    typedef struct {

        NTYPE type;
        int iVal;
        double dVal;
        bool bVal;
        const char* sVal;

        Lua::LuaValue GetLuaValue()
        {
            if(type == NTYPE::STRING)
            {
                return new Lua::LuaValue(std::string(sVal));
            }

            if(type == NTYPE::INTEGER)
            {
                return new Lua::LuaValue(iVal);
            }

            if(type == NTYPE::DOUBLE)
            {
                return new Lua::LuaValue(dVal);
            }

            if(type == NTYPE::BOOLEAN)
            {
                return new Lua::LuaValue(bVal);
            }

            return new Lua::LuaValue();
        }

        void Debug()
        {
            if(type == NTYPE::STRING)
            {
                printf("nval STR : %s \n", sVal);
                return;
            }

            if(type == NTYPE::INTEGER)
            {
                printf("nval INT : %d \n", iVal);
                return;
            }

            if(type == NTYPE::DOUBLE)
            {
                printf("nval DBL : %f \n", dVal);
                return;
            }

            if(type == NTYPE::BOOLEAN)
            {
                printf("nval BLD : %s \n", bVal ? "true" : "false");
                return;
            }

            printf("nval NULL\n");
        }
    } NValue;

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
    void InitDelegates()
    {
        isSetup = true;
    }
    Plugin::NValue* CallBridge(const char* key, void** args, int len)
    {
        return (Plugin::NValue*) this->bridge.CallBridge(key, args, len);
    }
    NetBridge GetBridge() {
        return this->bridge;
    }
    NValue* CreateNValueByLua(const Lua::LuaValue lVal)
    {
        if(lVal.IsBoolean())
        {
            NValue* nVal = new NValue;
            nVal->type = NTYPE::BOOLEAN;
            nVal->bVal = lVal.GetValue<bool>();
            return nVal;
        }

        if(lVal.IsInteger())
        {
            NValue* nVal = new NValue;
            nVal->type = NTYPE::INTEGER;
            nVal->iVal = lVal.GetValue<int>();
            return nVal;
        }

        if(lVal.IsNumber())
        {
            NValue* nVal = new NValue;
            nVal->type = NTYPE::DOUBLE;
            nVal->dVal = lVal.GetValue<double>();
            return nVal;
        }

        if(lVal.IsString())
        {
            NValue* nVal = new NValue;
            nVal->type = NTYPE::STRING;
            auto sVal = lVal.GetValue<std::string>();
            nVal->sVal = sVal.c_str();
            return nVal;
        }

        NValue* nVal = new NValue;
        nVal->type = NTYPE::NONE;
        return nVal;
    }
    Lua::LuaArgs_t CallLuaFunction(const char* LuaFunctionName, Lua::LuaArgs_t* Arguments);
};