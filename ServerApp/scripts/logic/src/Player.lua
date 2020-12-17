local Object=require("logic/src/Object");
local Player=Object:New("[Class] Player");
local ServiceType=require("ServiceType");
local config=require("GameConfig");
local Mysql=require("database/mysql_center");
local PlayerState=require("logic/const/PlayerState");
Mysql.Connect();

function Player:New(tcpSession,uname,utag)
    local instance = Object.New(self,uname);
    --socketinfo
    instance.__tcpSession=tcpSession;
    instance.__utag=utag;
    instance.__udpIp=nil;
    instance.__udpPort=0;
    --userinfo
    instance.Uname=uname;
    instance.Uface=0;
    instance.Unick="";
    instance.State=PlayerState.OnLine;
    --roomInfo
    instance.IsSubmit=false;
    instance.IsReady=false;
    instance.Room=nil;
    instance.SeatId=-1;
    instance.HeroId=-1;
    instance.ExpectedRoomType=-1;
    --game
    instance.SyncFrameId=0;

    return instance;
end

function Player:SetUdpAddr(udpIp,udpPort)
    self.__udpIp=udpIp;
    self.__udpPort=udpPort;
end

function Player:IsInRoom()
    return self.Room~=nil;
end

function Player:OnAddToRoom(room)
    self.Room=room;
    self.State=PlayerState.Matching;
    if config.enable_match_log then
        Debug.Log("Player["..self.Uname.."] enter Room["..self.Room.RoomId.."]");
    end
end

function Player:OnRemoveFromRoom(room)
    if config.enable_match_log then
        Debug.Log("Player["..self.Uname.."] leave Room["..self.Room.RoomId.."]");
    end
    self.Room=nil;
    self.SeatId=-1;
    self.HeroId=-1;
    self.SyncFrameId=0;
    self.IsReady=false;
    self.IsSubmit=false;
    self.State=PlayerState.OnLine;
end

function Player:UdpSend(cmdType,body)
	--玩家已经断线
	if not self.__tcpSession then
        Debug.LogWarning(self.Unick.." is offline")
		return;
	end

    if not self.__udpIp or self.__udpPort==0 then
        Debug.LogWarning(self.Unick.." Udp address is wrong")
		return;
	end

   Session.UdpSendPackage(self.__udpIp,self.__udpPort,{
    ServiceType.Logic,cmdType,0,body
   });
end

function Player:TcpSend(cmdType,body)
    -- Debug.Log("[Player.TcpSend] User "..self.Uname.."  cmdType "..cmdType);
    Session.SendPackage(
        self.__tcpSession,
        {
            ServiceType.Logic,
            cmdType,
            self.__utag,
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

function Player:OnLine()
    self.State=PlayerState.OnLine;
    if self.Room then
        self.Room:OnPlayerOnLine(self);
    end
end

function Player:OnOffLine()
    self.State=PlayerState.OffLine;
    if self:IsInRoom() then
        self.Room:OnPlayerOffLine(self);
    end
end

return Player;