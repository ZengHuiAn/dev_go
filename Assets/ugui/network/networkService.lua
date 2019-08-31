local conn = nil

local function Connect(host, port)
    if conn then
        conn:Close()
        conn = nil
    end
    conn = CS.Snake.Client()
    local result = conn:Connect(host, port)

    return result
end
local nextSN = 0

function createSerialNumber()
    nextSN = nextSN + 1
    return nextSN
end

local function Send(cmd, data, sn)

    fmt.Println("send--->>>",cmd,data,sn)
    if conn  then
        data = data or {}
        if sn == nil then
            sn = createSerialNumber()
        end
        conn:Send(cmd, sn, data)
    end
end


local function Read()
    if  ~conn then
        return
    end

    local valueTB = conn:Read()

end

return {
    Connect = Connect,
    Send = Send,
    Read = Read
}

