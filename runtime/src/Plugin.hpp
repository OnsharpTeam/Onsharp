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
    enum class NTYPE
    {
        NONE = 0,
        STRING = 1,
        DOUBLE = 2,
        INTEGER = 3,
        BOOLEAN = 4,
        TABLE = 5
    };

    struct NValue {

        NTYPE type = NTYPE::NONE;
        int iVal = 0;
        double dVal = 0;
        bool bVal = false;
        std::string sVal;
        Lua::LuaTable_t  tVal;

        void AddAsArg(Lua::LuaArgs_t* args)
        {
            if(type == NTYPE::STRING)
            {
                args->emplace_back(sVal);
                return;
            }

            if(type == NTYPE::INTEGER)
            {
                args->emplace_back(iVal);
                return;
            }

            if(type == NTYPE::DOUBLE)
            {
                args->emplace_back(dVal);
                return;
            }

            if(type == NTYPE::BOOLEAN)
            {
                args->emplace_back(bVal);
                return;
            }

            if(type == NTYPE::TABLE)
            {
                args->emplace_back(tVal);
                return;
            }
        }

        Lua::LuaValue GetLuaValue()
        {
            if(type == NTYPE::STRING)
            {
                return new Lua::LuaValue(sVal);
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

            if(type == NTYPE::TABLE)
            {
                return new Lua::LuaValue(tVal);
            }

            return new Lua::LuaValue();
        }

        void Debug() const
        {
            if(type == NTYPE::STRING)
            {
                Onset::Plugin::Get()->Log("nval STR : %s \n", sVal.c_str());
                return;
            }

            if(type == NTYPE::INTEGER)
            {
                Onset::Plugin::Get()->Log("nval INT : %d \n", iVal);
                return;
            }

            if(type == NTYPE::DOUBLE)
            {
                Onset::Plugin::Get()->Log("nval DBL : %f \n", dVal);
                return;
            }

            if(type == NTYPE::BOOLEAN)
            {
                Onset::Plugin::Get()->Log("nval BLD : %s \n", bVal ? "true" : "false");
                return;
            }

            Onset::Plugin::Get()->Log("nval NULL\n");
        }
    };

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
    }
    Plugin::NValue* CallBridge(const char* key, void** args, int len)
    {
        return (Plugin::NValue*) this->bridge.CallBridge(key, args, len);
    }
    NetBridge GetBridge() {
        return this->bridge;
    }
    NValue* CreatNValueByString(std::string val){
        NValue* nVal = new NValue;
        nVal->type = NTYPE::STRING;
        nVal->sVal = std::move(val);
        return nVal;
    }
    NValue* CreateNValueByLua(Lua::LuaValue lVal)
    {
        if(lVal.IsString())
        {
            NValue* nVal = new NValue;
            nVal->type = NTYPE::STRING;
            auto str  = lVal.GetValue<std::string>();
            const char* cstr = str.c_str();
            nVal->sVal = std::string(cstr);
            return nVal;
        }

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

        if(lVal.IsTable())
        {
            NValue* nVal = new NValue;
            nVal->type = NTYPE::TABLE;
            nVal->tVal = lVal.GetValue<Lua::LuaTable_t>();
            return nVal;
        }

        NValue* nVal = new NValue;
        nVal->type = NTYPE::NONE;
        return nVal;
    }
    Lua::LuaArgs_t CallLuaFunction(const char* LuaFunctionName, Lua::LuaArgs_t* Arguments);
    void ClearLuaStack();
};