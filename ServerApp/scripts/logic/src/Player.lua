local Object=require("logic/src/Object");
local Player=Object:New("[Class] Player");
local ServiceType=require("ServiceType");
local config=require("GameConfig");
local Mysql=require("database/mysql_center");
Mysql.Connect();

function Player:New(tcpSession,uname,utag,roomType)
    local instance = Object.New(self,uname);
    --socketinfo
    instance.TcpSession=tcpSession;
    instance.Utag=utag;
    --userinfo
    instance.Uname=uname;
    instance.Uface=0;
    instance.Unick="";
    --roomInfo
    instance.IsSubmit=false;
    instance.RoomId=-1;
    instance.SeatId=-1;
    instance.HeroId=-1;
    instance.ExpectedRoomType=roomType;

    return instance;
end

function Player:IsInRoom()
    return self.RoomId==-1;
end

function Player:OnAddToRoom(room)
    self.RoomId=room.RoomId;
    if config.enable_match_log then
        Debug.Log("Player["..self.Uname.."] enter Room["..self.RoomId.."]");
    end
end

function Player:OnRemoveFromRoom(room)
    if config.enable_match_log then
        Debug.Log("Player["..self.Uname.."] leave Room["..self.RoomId.."]");
    end
    self.RoomId=-1;
    self.SeatId=-1;
    self.HeroId=-1;
    self.IsSubmit=false;
end

function Player:TcpSend(cmdType,body)
    -- Debug.Log("[Player.TcpSend] User "..self.Uname.."  cmdType "..cmdType);
    Session.SendPackage(
        self.TcpSession,
        {
            ServiceType.Logic,
            cmdType,
            self.Utag,
            body,
        }
    );
end

--handler:matcher
function Player:GetMatchInfo(seatId,handler)
    self.SeatId=seatId;
    Mysql.GetMatcherInfoByUname(self.Uname,function(err,matchInfo)
        if err then
            Debug.LogError("[Player:GetMatchInfo MysqlError]"..err);
            handler(nil);
            return;
        end
        if not matchInfo then
            Debug.LogError("[Player:GetMatchInfo MysqlError] matchInfo is null");
            handler(nil);
            return;
        end
        matchInfo.seatId=self.SeatId;
        handler(matchInfo);
    end);
end

--handler:defaultHeroId
function Player:GetDefaultHeroId(handler)
    Mysql.GetDefaultHeroIdByUname(self.Uname,function(err,defaultHeroId)
        if err then
            Debug.LogError("[Player:GetDefaultHeroId MysqlError]"..err);
            handler(nil);
            return;
        end
        if not defaultHeroId then
            Debug.LogError("[Player:GetDefaultHeroId MysqlError] defaultHeroId is null");
            handler(nil);
            return;
        end
        handler(defaultHeroId);
    end);
end

return Player;