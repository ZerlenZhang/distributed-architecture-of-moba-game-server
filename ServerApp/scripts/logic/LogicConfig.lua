local CmdType=require("logic/const/CmdType");

roomInfos={}
roomInfos[CmdType.StartMatchReq]={
    Max=2,
}


return {
    udp_ip="121.196.178.141",
    udp_port=10000,
    room_infos=roomInfos,
    heroSelectTime=20,
    finishSelectDelay=2,
};