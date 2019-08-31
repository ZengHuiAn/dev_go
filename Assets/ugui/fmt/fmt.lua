-- 打印表的格式的方法
local function _sprinttb(tb, tabspace)
    tabspace =tabspace or ''
    local str =string.format(tabspace .. '{\n' )
    for k,v in pairs(tb or {}) do
        if type(v)=='table' then
            if type(k)=='string' then
                str =str .. string.format("%s%s =\n", tabspace..'  ', k)
                str =str .. _sprinttb(v, tabspace..'  ')
            elseif type(k)=='number' then
                str =str .. string.format("%s[%d] =\n", tabspace..'  ', k)
                str =str .. _sprinttb(v, tabspace..'  ')
            end
        else
            if type(k)=='string' then
                str =str .. string.format("%s%s = %s,\n", tabspace..'  ', tostring(k), tostring(v))
            elseif type(k)=='number' then
                str =str .. string.format("%s[%s] = %s,\n", tabspace..'  ', tostring(k), tostring(v))
            end
        end
    end
    str =str .. string.format(tabspace .. '},\n' )
    return str
end

function sprinttb(tb, tabspace)
        local function ss()
            return _sprinttb(tb, tabspace);
        end
        return setmetatable({}, {
            __concat = ss,
            __tostring = ss,
        });
end

function Println(...)
    local value = ...
    if type(value) == 'table' then
        print(sprinttb(value))
    else
        print(value)
    end

end


function Error(...)
    error(...,1)
end

return {
    Println = Println,
    Error  =Error
}

