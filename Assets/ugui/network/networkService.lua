local conn = nil


local function Read()

    if  conn == nil then
        return
    end
    StartCoroutine(function()
        while true
        do
            print("卡死？")
            WaitForSeconds(1)
            local valueTB = conn:Read()

        end
    end)
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






return {
    Connect = Connect,
    Send = Send,
    Read = Read
}

