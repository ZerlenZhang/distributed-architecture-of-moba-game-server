return {
    serviceType = 1,
    service = {
        OnSessionRecvCmd=function(s,msg)
            print(msg[3]);
            --如果是Protobuf就把内容解析后传给Lua
	        --{ 1： serviceType, 2: cmdType, 3: uTag, 4: body }
            print(msg[1]);
            print(msg[2]);
            local body = msg[4];
            print("body.name: ",body.name);
            print("body.email: ",body.email);
            print("body.age: ",body.age);


            --发送数据
             --第一个参数：session
            --第二个参数：表：{1： sType, 2: cType, 3: uTag, 4: body}

            local package={1,2,0,{status=200}}
            Session.SendPackage(s,package);
        end,
        OnSessionDisconnected=function(s)
            local ip,port=Session.GetAddress(s);
            print("IP: ",ip,"Port: ",port);
        end,
    };
};