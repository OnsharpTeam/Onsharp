local importedPackages = {}

function Onsharp_ImportPackage(importId, packageName)
    if importedPackages[importId] ~= nil then
        return
    end

    importedPackages[importId] = ImportPackage(packageName)
end

function Onsharp_InvokePackage(importId, funcName, ...)
    importedPackages[importId][funcName](...)
end

function Onsharp_RegisterCommand(pluginId, commandName)
    AddCommand(commandName, function(playerId, ...)
        local commandArgs = {...};
        local line = "";
        for _, v in ipairs(commandArgs) do
            line = line .. " " .. tostring(v);
        end

        local args = {};
        args[1] = pluginId;
        args[2] = playerId;
        args[3] = commandName;
        args[4] = line;
		
        CallBridge("call-command", args);
    end)
end

function Onsharp_RegisterCommandAlias(pluginId, commandName, alias)
    AddCommand(alias, function(playerId, ...)
        local commandArgs = {...};
        local line = "";
        for _, v in ipairs(commandArgs) do
            line = line .. " " .. tostring(v);
        end

        local args = {};
        args[1] = pluginId;
        args[2] = playerId;
        args[3] = commandName;
        args[4] = line;
		
        CallBridge("call-command", args);
    end)
end

function Onsharp_RegisterRemoteEvent(pluginId, eventName)
    AddRemoteEvent(eventName, function(playerId, ...)
        local args = {};
        args[1] = playerId;
        args[2] = pluginId;
        args[3] = eventName;

        local remoteArgs = {...};
		local idx = 4
        for _, v in ipairs(remoteArgs) do
            args[idx] = v;
			idx = idx + 1
        end

        CallBridge("call-remote", args);
    end)
end
function Onsharp_Delay(id, millis)
    Delay(millis, function ()
        local args = {}
        args[1] = id
        CallBridge("call-delay", args)
    end)
end

function Onsharp_CreateTimer(id, interval)
    return CreateTimer(function ()
        local args = {}
        args[1] = id
        CallBridge("call-timer", args)
    end, interval)
end

InitRuntimeEntries()

local function CallBridgedEvent(eventType, ...)
    local args = {};
    args[1] = eventType;
    
    local remoteArgs = {...};
    for _, v in ipairs(remoteArgs) do
        args[#args + 1] = v;
    end

    return CallBridge("call-event", args);
end

-- START SERVER EVENTS --
AddEvent("OnPlayerQuit", function(playerId)
    return CallBridgedEvent(0, playerId)
end)

AddEvent("OnPlayerChat", function(playerId, text_)
    return CallBridgedEvent(1, playerId, text_)
end)

AddEvent("OnPlayerChatCommand", function(playerId, command_, exists_)
    return CallBridgedEvent(2, playerId, command_, exists_)
end)

AddEvent("OnPlayerJoin", function(playerId)
    return CallBridgedEvent(3, playerId)
end)

AddEvent("OnPlayerPickupHit", function(playerId, pickup_)
    return CallBridgedEvent(4, playerId, pickup_)
end)

AddEvent("OnPackageStart", function()
    return CallBridgedEvent(6)
end)

AddEvent("OnPackageStop", function()
    return CallBridgedEvent(7)
end)

AddEvent("OnGameTick", function(delta_)
    return CallBridgedEvent(8, delta_)
end)

AddEvent("OnClientConnectionRequest", function(ip_, port_)
    return CallBridgedEvent(9, ip_, port_)
end)

AddEvent("OnNPCReachTarget", function(npc_)
    return CallBridgedEvent(10, npc_)
end)

AddEvent("OnNPCDamage", function(npc_, type_, amount_)
    return CallBridgedEvent(11, npc_, type_, amount_)
end)

AddEvent("OnNPCSpawn", function(npc_)
    return CallBridgedEvent(12, npc_)
end)

AddEvent("OnNPCDeath", function(npc_)
    return CallBridgedEvent(13, npc_)
end)

AddEvent("OnNPCStreamIn", function(npc_, player_)
    return CallBridgedEvent(14, player_, npc_)
end)

AddEvent("OnNPCStreamOut", function(npc_, player_)
    return CallBridgedEvent(15, player_, npc_)
end)

AddEvent("OnPlayerEnterVehicle", function(player_, vehicle_, seat_)
    return CallBridgedEvent(16, player_, vehicle_, seat_)
end)

AddEvent("OnPlayerLeaveVehicle", function(player_, vehicle_, seat_)
    return CallBridgedEvent(17, player_, vehicle_, seat_)
end)

AddEvent("OnPlayerStateChange", function(player_, newstate_, oldstate_)
    return CallBridgedEvent(18, player_, newstate_, oldstate_)
end)

AddEvent("OnVehicleRespawn", function(vehicle_)
    return CallBridgedEvent(19, vehicle_)
end)

AddEvent("OnVehicleStreamIn", function(vehicle_, player_)
    return CallBridgedEvent(20, player_, vehicle_)
end)

AddEvent("OnVehicleStreamOut", function(vehicle_, player_)
    return CallBridgedEvent(21, player_, vehicle_)
end)

AddEvent("OnPlayerServerAuth", function(player_)
    return CallBridgedEvent(22, player_)
end)

AddEvent("OnPlayerSteamAuth", function(player_)
    return CallBridgedEvent(23, player_)
end)

AddEvent("OnPlayerDownloadFile", function(player_, file_, checksum_)
    return CallBridgedEvent(24, player_, file_, checksum_)
end)

AddEvent("OnPlayerStreamIn", function(player_, other_)
    return CallBridgedEvent(25, player_, other_)
end)

AddEvent("OnPlayerStreamOut", function(player_, other_)
    return CallBridgedEvent(26, player_, other_)
end)

AddEvent("OnPlayerSpawn", function(player_)
    return CallBridgedEvent(27, player_)
end)

AddEvent("OnPlayerDeath", function(player_, killer_)
    return CallBridgedEvent(28, player_, killer_)
end)

AddEvent("OnPlayerWeaponShot", function(player_, weapon_, hittype_, hitid, hitX, hitY, hitZ, startX, startY, startZ, normalX, normalY, normalZ)
    return CallBridgedEvent(29, player_, weapon_, hittype_, hitid, hitX, hitY, hitZ, startX, startY, startZ, normalX, normalY, normalZ)
end)

AddEvent("OnPlayerDamage", function(player_, type_, amount_)
    return CallBridgedEvent(30, player_, type_, amount_)
end)

AddEvent("OnPlayerInteractDoor", function(player_, door_, bWantsOpen)
    return CallBridgedEvent(31, player_, door_, bWantsOpen)
end)

AddEvent("OnVehicleDamage", function(vehicle, healthDamage, damageIndex, damageAmount)
    return CallBridgedEvent(33, vehicle, healthDamage, damageIndex, damageAmount)
end)

AddEvent("OnConsoleInput", function(input)
    return CallBridgedEvent(34, input)
end)

AddEvent("OnDoorDestroyed", function(id)
    return CallBridgedEvent(35, id)
end)

AddEvent("OnNPCDestroyed", function(id)
    return CallBridgedEvent(36, id)
end)

AddEvent("OnObjectDestroyed", function(id)
    return CallBridgedEvent(37, id)
end)

AddEvent("OnPickupDestroyed", function(id)
    return CallBridgedEvent(38, id)
end)

AddEvent("OnText3DDestroyed", function(id)
    return CallBridgedEvent(39, id)
end)

AddEvent("OnPlayerChangeDimension", function(id, new_dim, old_dim) 
    return CallBridgedEvent(40, id, new_dim, old_dim)
end)

AddEvent("OnVehicleChangeDimension", function(id, new_dim, old_dim) 
    return CallBridgedEvent(41, id, new_dim, old_dim)
end)

AddEvent("OnText3DChangeDimension", function(id, new_dim, old_dim) 
    return CallBridgedEvent(42, id, new_dim, old_dim)
end)

AddEvent("OnPickupChangeDimension", function(id, new_dim, old_dim) 
    return CallBridgedEvent(43, id, new_dim, old_dim)
end)

AddEvent("OnObjectChangeDimension", function(id, new_dim, old_dim) 
    return CallBridgedEvent(44, id, new_dim, old_dim)
end)

AddEvent("OnNPCChangeDimension", function(id, new_dim, old_dim) 
    return CallBridgedEvent(45, id, new_dim, old_dim)
end)

AddEvent("OnObjectCreated", function(id) 
    return CallBridgedEvent(46, id)
end)

AddEvent("OnVehicleCreated", function(id) 
    return CallBridgedEvent(47, id)
end)

AddEvent("OnText3DCreated", function(id) 
    return CallBridgedEvent(48, id)
end)

AddEvent("OnPickupCreated", function(id) 
    return CallBridgedEvent(49, id)
end)

AddEvent("OnNPCCreated", function(id) 
    return CallBridgedEvent(50, id)
end)

AddEvent("OnDoorCreated", function(id) 
    return CallBridgedEvent(51, id)
end)

AddEvent("OnObjectStopMoving", function(id) 
    return CallBridgedEvent(52, id)
end)

-- END SERVER EVENTS --