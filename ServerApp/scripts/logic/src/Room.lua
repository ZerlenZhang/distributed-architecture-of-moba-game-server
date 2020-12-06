local Object=require("logic/src/Object");
local List=require("logic/src/List");
local CmdType=require("logic/const/CmdType");
local LogicConfig=require("logic/LogicConfig");

local Room=Object:New("[Class] Room");

Room.__roomIndex=0;

--new constructor
function Room:New(roomType, max)
    Room.__roomIndex=Room.__roomIndex+1;

    local instance=Object.New(self, "[Room:"..Room.__roomIndex.."]");


    --new instance properies
    instance.RoomId=Room.__roomIndex;
    instance.RoomType=roomType;
    instance.Max=max;
    instance.PlayerList=List:New(instance.name.."PlayerList");
    instance.__leftTime=0;
    instance.__selectTimer=nil;

    --game
    instance.__gameLoopTimer=nil;
	self.__frameId=1;--当前帧id
	self.__allFrames={};--所有帧操作
    self.__nextFrame=--当前帧操作
    {
		frameId = self.__frameId,
		inputs = {},
	};

    return instance;
end

--override
function Room:Print()
    --call base method
    --Object.Print(self);
    Debug.Log("[Room.Instance.Method.Print]")
end

--new methods
function Room:LogRoomInfo()
    Debug.Log("[Room]RoomId-"..self.RoomId..":RoomType-"..self.RoomType..":Max-"..self.Max..":Player-"..self.PlayerList.Count().."/"..self.Max);
end

function Room:BroadMessage(cmdType,body,ignore)
    self.PlayerList:Foreach(function(index,player)
        if ignore~=player then
            player:TcpSend(cmdType,body)
        end
    end)
end

function Room:AddPlayer(player)
    --add player
    player:OnAddToRoom(self);
    self.PlayerList:Add(player);

    --notice all players
    self:BroadMessage(CmdType.AddMatcherTick);

    --check if finish
    if self:IsFull() then
        self:StartHeroPick();
    end
end

function Room:RemovePlayer(player)
    player:OnRemoveFromRoom(self);
    self.PlayerList:Remove(player);

    self:BroadMessage(CmdType.RemoveMatcherTick);
end

function Room:PlayerCount()
    return self.PlayerList:Count();
end

function Room:IsFull()
    return self.PlayerList:Count()==self.Max;
end

--开始英雄选择
function Room:StartHeroPick()
    Debug.Log("Room["..self.RoomId.."] start hero pick");
    local matchinfoArr=List:New("matchInfoArr:"..self.RoomId);
    local finished=0;
    self.PlayerList:Foreach(function(index,player)
        player:GetMatchInfo(index,
        function(matchInfo)
            matchinfoArr:Add(matchInfo);
            finished=finished+1;
            --信息获取完成，广播消息匹配完成
            if finished==self.Max then
                self:BroadMessage(CmdType.FinishMatchTick,{
                    matchers=matchinfoArr:Data(),
                    heroSelectTime=LogicConfig.heroSelectTime,
                    roomId = self.RoomId,
                });
                --英雄选择倒计时
                self.__leftTime=LogicConfig.heroSelectTime;
                self.__selectTimer=Timer.Repeat(function()
                    self.__leftTime=self.__leftTime-1;
                    if self.__leftTime==0 then
                        self:ForceStartGame();
                    else
                        self:BroadMessage(CmdType.UpdateSelectTimer,{current=self.__leftTime});
                    end
                end,1000,self.__leftTime-1,1000);
            end
        end);
    end);
end

--有人时间到了还没选好
function Room:ForceStartGame()
    Timer.Cancel(self.__selectTimer);
    self.__selectTimer=nil;
    local unfinishedPlayers=self.PlayerList:Where(function(player)return player.HeroId==-1;end);
    local finished=0;
    local uname_heroId={};
    unfinishedPlayers:Foreach(function(index, player)
        player:GetDefaultHeroId(function(defaultHeroId)
            finished=finished+1;
            uname_heroId[player.Uname]=defaultHeroId;

            --已经获取所有未选中的人的默认英雄
            if finished==unfinishedPlayers:Count() then
                local selectHeroResList=List:New();
                self.PlayerList:Foreach(function(index,player)
                    --锁定英雄
                   player.IsSubmit=true;
                   --设置默认英雄
                   if player.HeroId==-1 then
                        player.HeroId=uname_heroId[player.Uname];
                   end
                   selectHeroResList:Add({
                        seatId = player.SeatId,
                        hero_id = player.HeroId,
                   });
                end);
                self:BroadMessage(CmdType.ForceSelect,{
                    selectResults = selectHeroResList:Data(),
                });
                Timer.Once(function() self:StartLoadGame(); end, LogicConfig.finishSelectDelay*1000);
            end
        end);
    end);
end

--有人锁定了英雄
function Room:OnMatcherSubmit(uname)
    local submitCount=0;
    local player=nil;
    self.PlayerList:Foreach(function(index, player)
        if player.Uname==uname then
            if not player.IsSubmit then
                --锁定英雄
                player.IsSubmit=true;
                self:BroadMessage(CmdType.SubmitHeroRes,{seatId=player.SeatId});
            end
            submitCount=submitCount+1;
        elseif player.IsSubmit then
            submitCount=submitCount+1;
        end
    end);
    --全部锁定就开始游戏
    if submitCount==self:PlayerCount() then
        Timer.Cancel(self.__selectTimer);
        self.__selectTimer=nil;
        Timer.Once(function() self:StartLoadGame(); end, LogicConfig.finishSelectDelay*1000);
    end
end

--开始加载游戏
function Room:StartLoadGame()
    self:BroadMessage(CmdType.StartLoadGame);
end

--有玩家加载完毕，就绪
function Room:OnPlayerReady(player)
    player.IsReady=true;
    if self.PlayerList:All(function(player)return player.IsReady;end) then
        self:StartGame();
    end
end

--开始帧同步
function Room:StartGame()
    Debug.Log("Room["..self.RoomId.."] game start");
   local startGameRes={
       gameTime = LogicConfig.room_infos[CmdType.StartMatchReq].GameTime,
       randSeed = math.floor(math.random()*1000+10),
       logicFrameDeltaTime = LogicConfig.logicFrameDeltaTime,
       startGameDelay = LogicConfig.startGameDelay,
   };
   self:BroadMessage(CmdType.StartGameRes,startGameRes);
   self.__gameLoopTimer=Timer.Repeat(
	function()
		self:FrameSyncLoop();
	end,LogicConfig.startGameDelay,-1,LogicConfig.logicFrameDeltaTime);
end

--帧同步主循环
function Room:FrameSyncLoop()
    self.__allFrames[self.__frameId]=self.nextFrame;
    self.__frameId=self.__frameId+1;

    self.PlayerList:Foreach(function(_,player)
        --发送尚未同步的帧
        local frames = {};

        for i = (player.SyncFrameId+1), #self.__allFrames do
            table.insert(frames,self.__allFrames[i]);
        end
        --	print("player.SyncFrameId:"..player.SyncFrameId.."self.__frameId"..self.__frameId.."#self.__allFrames:"..#self.__allFrames);
        player:UdpSend(CmdType.LogicFramesToSync,{
            frameId=self.__frameId,
            unsyncFrames=frames,
        });
    end);

    self.nextFrame=
    {
		frameId = self.__frameId,
		inputs = {},
	};
end

--接受收到的输入
function Room:TakeFrameInput(frameId, seatId,inputs)
	--是否过时
	if frameId<=self.__frameId then
		return;
	end
	--是否内容无效
	if #inputs <= 0 then
		Debug.LogError("get empty NextFrameOpts");
		return;
	end
	--是否不对应玩家
    if seatId<1 or seatId>self:PlayerCount() then
        Debug.LogError("unexpected seatId: "..seatId);
        return;
    end

    --更新客户端同步到帧数
	if player.SyncFrameId< frameId-1 then
		player.SyncFrameId=frameId-1;
	end

	--加入到当前帧操作
	for k, opt in pairs(inputs) do
		table.insert(self.__nextFrame.inputs,opt);
	end
end

return Room;