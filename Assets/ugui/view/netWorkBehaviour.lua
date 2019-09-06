local netWork = require('network.networkService')

local View = {}

function View:Start()

end

function View:Update()
    netWork.Read()
end

function View:OnDestroy()
    netWork.Close()
end


return View