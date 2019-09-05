local conn = nil


local function Read()

    if  conn == nil then
        return
    end
    local valueTB = conn:BegainRead(0)
end


---Connect
---@param host string
---@param port string
local function Connect(host, port)
    if conn then
        conn:Close()
        conn = nil
    end
    conn = CS.Snake.Client()
    local result = conn:Connect(host, port)
    Read();

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

local function Close()
    if conn then
        conn:Close()
        conn = nil
    end
end


local function ReadBuffer()
    if conn and conn.Connected then
       local reader = conn:GetBuffer()
        fmt.Println(reader)
        return reader
    end
    return nil
end



return {
    Connect = Connect,
    Send = Send,
    Read = ReadBuffer,
    Close = Close
}

