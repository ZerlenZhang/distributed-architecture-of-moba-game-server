local ServiceType = require("ServiceType");

local cmdNameMap={};
cmdNameMap[ServiceType.Logic]={
    UserLostConn = 0,
    LoginLogicReq = 1,
    LoginLogicRes = 2,
    UdpTestReq = 3,
    UdpTestRes = 4,
    StartMatchReq = 5,
    StartMatchRes = 6,
    AddMatcherTick = 7,
    RemoveMatcherTick = 8,
    FinishMatchTick = 9,
    StopMatchReq = 10,
    StopMatchRes = 11,
    SelectHeroReq = 12,
    SelectHeroRes = 13,
    SubmitHeroReq = 14,
    SubmitHeroRes = 15,
    UpdateSelectTimer = 16,
    ForceSelect = 17,
    StartLoadGame = 18,
    StartGameReq = 19,
    StartGameRes = 20,
    NextFrameInput = 21,
    LogicFramesToSync = 22,
    InitUdpReq = 23,
    InitUdpRes = 24,
    StartStoryReq = 25,
    StartStoryRes = 26,
    StartMultiReq = 27,
};
return cmdNameMap;