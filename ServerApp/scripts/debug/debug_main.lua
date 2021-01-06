local Object=require("logic/src/Object");

Debug.LogInit("../logs/debug","debug",true);
local o1=Object:New("o1");
local o2 =Object:New("o2");
o1:Print();
o2:Print();

local Room=require("logic/src/Room");
local r1=Room:New(0,10);
r1:LogRoomInfo();

local List=require("logic/src/List");
local roomList=List:New("Room");
roomList:Add(o1);
roomList:Add(o2);
Debug.Log("ListSize:"..roomList:Count());
roomList:Foreach(function(i,v)
    Debug.Log("Foreach: "..i.." "..v.name);
end)
roomList:Remove(o1);
Debug.Log("ListSize:"..roomList:Count());
roomList:Foreach(function(i,v)
    Debug.Log("Foreach: "..i.." "..v.name);
end)

for i=0,1 do
    print(i);
end