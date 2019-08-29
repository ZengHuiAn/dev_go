
UnityEngine = CS.UnityEngine
local tb = CS.Snake.LuaConvert.TestConvertTable();
fmt = require("fmt.fmt")
fmt.Println(#tb)
fmt.Println("测试入口")
---@type UnityEngine.GameObject
local go

local network = require("network.networkService")
setmetatable(_G, {__index=function(_, k)
    fmt.Error("GLOBAL NAME", k, "NOT EXISTS", debug.traceback())
end, __newindex = function(t, k, v)
    fmt.Error("SET GLOBAL NAME", k, v, debug.traceback())
    rawset(t, k, v);
end})




