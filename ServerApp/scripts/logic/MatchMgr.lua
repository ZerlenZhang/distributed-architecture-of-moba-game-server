local List=require("logic/src/List");
local Room=require("logic/src/Room");
local Player=require("logic/src/Player");
local LogicConfig=require("logic/LogicConfig");
local CmdType=require("logic/const/CmdType");
local Respones=require("Respones");
local config=require("GameConfig");

local roomType_RoomList={}
local uname_Player={};

--确保有房间
local function GetRoom(roomType)
    --create roomList if not this roomType
    if not roomType_RoomList[roomType] then
        roomType_RoomList[CmdType.StartMatchReq]=List:New("[RoomType:"..roomType.."]RoomList");
    end
    --find an empty room, if not, create one
    local roomList=roomType_RoomList[CmdType.StartMatchReq];
    local room=roomList:FindFirst(function(room)return not room:IsFull();end);
    if not room then
        --create room
        room=Room:New(roomType,LogicConfig.room_infos[roomType].Max);
        roomList:Add(room);
    end
    return room;
end

--自带报错
local function GetRoomByTypeAndId(roomType, roomId)
    if not roomType_RoomList[roomType] then
        Debug.LogError("Unexpected roomType:"..roomType);
        return;
    end

    local roomList=roomType_RoomList[roomType];
    local room=roomList:FindFirst(function(room)return room.RoomId==roomId;end);
    if not room then
        Debug.LogError("Unexpected roomId:"..roomId);
        return;
    end
    return room;

end

local function on_player_try_match(s,utag,roomType,body)

    local uname=body.uname;
    local player=uname_Player[uname];
    if not player then
        player=Player:New(s,uname,utag,roomType);
        uname_Player[uname]=player;
    end
    local room=GetRoom(roomType);

    --start match feedback
    player:TcpSend(CmdType.StartMatchRes,{
        current=room:PlayerCount(),
        max=room.Max;
    })

    --add player to room
    room:AddPlayer(player);
end

local function on_player_stop_match(uname)
    if not uname_Player[uname] then
        Debug.LogError("unexpected uname: "..uname);
        return;
    end
    local player=uname_Player[uname];
    local room=player.Room;
    if not room then
        Debug.LogError("player "..uname.." is not in any room");
        return;
    end
    if room:IsFull() then
        --cant't leave
        player:TcpSend(CmdType.StopMatchRes,{status=Respones.InvalidOperate});
    else
        player:TcpSend(CmdType.StopMatchRes,{status=Respones.Ok});
        room:RemovePlayer(player);
    end

end

local function on_player_select_hero(uname, heroId)
    if not uname_Player[uname] then
        Debug.LogError("unexpected uname: "..uname);
        return;
    end
    local player=uname_Player[uname];
    local room=player.Room;
    if not room then
        Debug.LogError("player "..uname.." is not in any room");
        return;
    end

    if config.enable_match_log then
        Debug.Log(uname.." select hero["..heroId.."]");
    end

    player.HeroId=heroId;
    room:BroadMessage(CmdType.SelectHeroRes,{
        seatId = player.SeatId,
        hero_id = player.HeroId,
    });
end

local function on_player_submit_hero(uname)
    if not uname_Player[uname] then
        Debug.LogError("unexpected uname: "..uname);
        return;
    end

    local player=uname_Player[uname];
    local room=player.Room;
    if not room then
        Debug.LogError("player "..uname.." is not in any room");
        return;
    end
    if config.enable_match_log then
        Debug.Log(uname.." submit");
    end

    room:OnMatcherSubmit(uname);
end

local function on_player_try_start_game(uname)
    if not uname_Player[uname] then
        Debug.LogError("unexpected uname: "..uname);
        return;
    end
    local player=uname_Player[uname];
    local room=player.Room;
    if not room then
        Debug.LogError("player "..uname.." is not in any room");
        return;
    end
    room:OnPlayerReady(player);
end

local function on_take_frame_input(roomType, roomId, frameId,seatId,inputs)
    local room=GetRoomByTypeAndId(roomType,roomId);
    if not room then
        Debug.LogError("unexcepted room: type-"..roomType..", roomId-"..roomId);
    end
    room:TakeFrameInput(frameId,seatId,inputs);
end

return {
    OnPlayerTryMatch=on_player_try_match,
    OnPlayerTryStopMatch=on_player_stop_match,
    OnPlayerSelectHero=on_player_select_hero,
    OnPlayerSubmitHero=on_player_submit_hero,
    OnPlayerTryStartGame=on_player_try_start_game,
    OnTakeFrameInput=on_take_frame_input,
}