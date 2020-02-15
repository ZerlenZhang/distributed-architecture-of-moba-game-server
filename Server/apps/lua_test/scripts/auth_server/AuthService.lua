return   {
    OnSessionRecvCmd=function(s,msg)
    	print(msg[1],msg[2],msg[3]);
    end,
    OnSessionDisconnected=function(s)
    end,
};