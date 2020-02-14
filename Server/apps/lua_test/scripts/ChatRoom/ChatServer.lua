local sessionSet={};

function broadcast_except(msg,exceptSession)
    print("broad...")
    for i = 1,#sessionSet do 
        if exceptSession ~= sessionSet[i] then
            print("broad !",msg[2]);
            Session.SendPackage(sessionSet[i],msg);
        end
    end
end


function OnRecvLoginReq(s)
    --当前是否已经在集合中
    for i = 1,#sessionSet do 
        if s == sessionSet[i] then
            local pk = {2,2,0,{status = -1}};
            Session.SendPackage(s,pk);
            return;
        end
    end

    --加入当前集合
    --print("server welcome");
    table.insert(sessionSet,s);

    local pk = {2,2,0,{status = 1}};
    Session.SendPackage(s,pk);
    local s_ip,s_port=Session.GetAddress(s);
    local msg={2,7,0,{ip=s_ip,port=s_port}};
    broadcast_except(msg,s);
end

function OnRecvExitReq(s)
    for i = 1,#sessionSet do 
        if s == sessionSet[i] then
            table.remove(sessionSet,i);
            --顺利离开聊天室
            local ip,port=Session.GetAddress(s);
            print(ip,":",port," leave chat_room");

            Session.SendPackage(s,{2,4,0,{status = 1}});

            local s_ip,s_port=Session.GetAddress(s);
            local msg={2,8,0,{ip=s_ip,port=s_port}};
            broadcast_except(msg,s);
            return;
        end
    end
    
    --点击离开的时候已经不再聊天室了
    Session.SendPackage(s,{2,4,0,{status = -1}});
end

function OnRecvSendReq(s,msg)
    for i = 1,#sessionSet do 
        if s == sessionSet[i] then
            --顺利发送消息
            --print("server get data");
            Session.SendPackage(s,{2,6,0,{status = 1}});

            local s_ip,s_port=Session.GetAddress(s);
            local m={2,9,0,{ip=s_ip,port=s_port,content=msg[4].content}};
            broadcast_except(m,s);
            return;
        end
    end
    --发送失败
    --print("failed to send");
    Session.SendPackage(s,{2,6,0,{status = -1}});
end


return  {
    serviceType = 2,
    service = {
        OnSessionRecvCmd=function(s,msg)
            local ctype=msg[2];
            if ctype==1 then
                OnRecvLoginReq(s);
            elseif ctype==3 then
                OnRecvExitReq(s);                
            elseif ctype==5 then
                OnRecvSendReq(s,msg);
            end
        end,
        OnSessionDisconnected=function(s)
            local ip,port=Session.GetAddress(s);
            print(ip,":",port," leave chat_room");
            for i = 1,#sessionSet do 
                if s == sessionSet[i] then
                    table.remove(sessionSet,i);
                    local s_ip,s_port=Session.GetAddress(s);
                    local msg={2,8,0,{ip=s_ip,port=s_port}};
                    broadcast_except(msg,s);
                    return;
                end
            end
        end,
    };
};