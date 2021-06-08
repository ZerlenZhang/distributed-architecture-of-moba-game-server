Debug.LogInit("ClientLogs","client",true);
Debug.Log("hello");
Netbus.TcpConnect("121.196.178.141",6080,
		--链接成功的回调
        function(session)
            if session then
                print("succeed to connect to [121.196.178.141:6080]");
                
                return;
            end
            print("failed to connect to [121.196.178.141:6080]")
        end);
