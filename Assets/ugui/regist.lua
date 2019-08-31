-- 这个文件是初始化执行的文件
-- 主要放一些 通用的全局方法

--[[
    命名空间初始化
]]

UE = UnityEngine
UI = UE.UI



GObject = UE.GameObject

--[[
    Unity 中的 Button 的所有事件
]]
UIButton = UI.Button


function GetObjComponent( obj , behaviour )
    local behavi = nil

    if typeof(obj) ~= typeof(behaviour) then
        behavi = obj:GetComponent(typeof(behaviour))
    else
        behavi = obj
    end

    return behavi
end


function GetGObjectBtn( obj)
    local btn  = GetObjComponent(obj,UIButton)
    

    return btn
end


function RegistButtonEvent(obj,func)
    local btn  = GetGObjectBtn(obj)

    if btn then
        btn.onClick:AddListener(func)
        return true
    end

    return false
end


--[[
    UI Text
]]


UIText = UI.Text

function ChangeUIText(obj,content)
    local txt  = GetObjComponent(obj,UIText)
    if txt then
        txt.text = content
        return true
    end

    return false
end