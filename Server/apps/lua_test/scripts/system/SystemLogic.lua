local sType = require("ServiceType");
local cmdType = require("system/Const/CmdType");
local responce = require("Respones");
local mobaConfig = require("MobaConfig");

local mysql = require("datebase/mysql_game");


function Test( err ,uid)
                    print("hander in UpdateLoginBonuesStatus");
                    if err then
                        Debug.LogError(err);
                        local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                        { 
                            status = responce.SystemError
                        }};
                        Session.SendPackage(s,retMsg);    
                        return;                    
                    end

                    mysql.AddCoin1(uid,bonuesInfo.bonues,
                        function ( err )
                            
                            if err then
                                Debug.LogError(err);
                                local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                                { 
                                    status = responce.SystemError
                                }};
                                Session.SendPackage(s,retMsg);    
                                return;                    
                            end

                            local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                            { 
                                status = responce.OK,
                            }};
                            Session.SendPackage(s,retMsg);    
                            return;   
                        end);


end

--检查每日登陆奖励
--handler: err,bonuesInfo
function check_login_bonues( uid,handler )
    mysql.GetBonuesInfo(uid,
        function( err,bonuesInfo )
            if err then
                handler(err,nil);
                return;
            end
            --用户第一次登陆
            if bonuesInfo==nil then
                --插入数据
                mysql.InsertBonuesInfo(uid,
                    function ( err )
                        if err then
                            handler(err,nil);
                            return;
                        end

                        --重新检查
                        check_login_bonues(uid,handler);
                        return;
                    end)

                return;
            end

            print("check_login_bonues");

            --更新发放奖励
            if bonuesInfo.bonues_time<Utils.Today() then
                --发放
                --执行奖励逻辑
                if bonuesInfo.bonues_time>Utils.Yesterday() then
                    --连读登陆
                    bonuesInfo.days=bonuesInfo.days+1;
                else
                    --重新开始计算
                    bonuesInfo.days=1;
                end

                --登陆奖励循环
                if bonuesInfo.days>#mobaConfig.login_bonues then
                    bonuesInfo.days=1;
                end


                bonuesInfo.status=0;
                bonuesInfo.bonues_time=Utils.TimeStamp();
                bonuesInfo.bonues=mobaConfig.login_bonues[bonuesInfo.days];

                --更新数据库
                mysql.SetBonuesInfo(uid,bonuesInfo,
                    function ( err )
                        if err then
                            handler(err,nil);
                            return;
                        end


                        handler(nil,bonuesInfo);
                    end)
            end

            handler(nil,bonuesInfo);
        end)    
end

--获取游戏信息
function get_ugame_info( s,req )
    if nil==req then
        Debug.LogError("WTF??");
        return;
    end
	local uid = req[3];

    --获取信息
	mysql.GetUgameInfo(uid,
		function ( err,uinfo )

			if err then
                Debug.LogError(err);
				local retMsg = {sType.System,cmdType.eGetMobaInfoRes,uid,
    			{ 
    				status = responce.SystemError
    			}};
    			Session.SendPackage(s,retMsg);
    			return;
    		end

			--没有查到对应信息
    		if uinfo==nil then
    			--就添加进数据库
    			mysql.InsertUgameInfo(uid,function(err)
    				if err then
    					--告诉客户端某个错误信息
    					print("Insert err",err);
    					local retMsg =  {sType.System,cmdType.eGetMobaInfoRes,uid,
    						{
                            	status = responce.SystemError
    						}};
    					Session.SendPackage(s,retMsg);
    					return;
    				end
    				--重新登陆
    				get_ugame_info(s,msg);
    				return;
    			end)
    			return;
    		end

    		--找到对应游戏数据
    		if uinfo.ustatus~=0 then
    			--游客账号被查封
    			print("this key has been frozen");
    			local retMsg =   {sType.System,cmdType.eGetMobaInfoRes,uid,
    				{ 
    					status = responce.UserIsFreeze
    				}};
    			Session.SendPackage(s,retMsg);
    			return;
    		end

            --print("Get info success");

            --检测登陆奖励
            check_login_bonues(uid,
                function( err,bonuesInfo )
                    if err then
                        Debug.LogError(err);
                        local retMsg = {sType.System,cmdType.eGetMobaInfoRes,uid,
                        { 
                            status = responce.SystemError
                        }};
                        Session.SendPackage(s,retMsg);
                        return;
                    end

                    --返回客户端

                    local retMsg = {sType.System,cmdType.eGetMobaInfoRes,uid,
                        { 
                            status = responce.OK,
                            uinfo = {
                                ucoin_1=uinfo.ucoin_1,
                                ucoin_2=uinfo.ucoin_2,
                                uexp=uinfo.uexp,
                                uvip=uinfo.uvip,
                                uitem_1=uinfo.uitem_1,
                                uitem_2=uinfo.uitem_2,
                                bonues_status=bonuesInfo.status,
                                bonues=bonuesInfo.bonues,
                                days=bonuesInfo.days,
                            },
                        }};
                    --print("send utag: ",utag);
                    Session.SendPackage(s,retMsg);
                end)
		end)
end

--处理得到每日奖励的消息
function recv_login_bonues( s,req )
    local uid =req[3];
    print("flag 1");
        --获取信息
    mysql.GetBonuesInfo(uid,
        function ( err,bonuesInfo )
            print("flag 2");
            if err then
                Debug.LogError(err);
                local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                { 
                    status = responce.SystemError
                }};
                Session.SendPackage(s,retMsg);
                return;
            end

            print("flag 3");
            if nil==bonuesInfo or bonuesInfo.status~=0 then
                print(bonuesInfo,bonuesInfo.status);
                local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                { 
                    status = responce.InvalidOprate
                }};
                Session.SendPackage(s,retMsg);
                return;
            end

            print("flag 4");
            --有奖励可以领取
            mysql.UpdateLoginBonuesStatus(uid,function ( err )
                    if err then
                        Debug.LogError(err);
                        local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                        { 
                            status = responce.SystemError
                        }};
                        Session.SendPackage(s,retMsg);    
                        return;                    
                    end

                    mysql.AddCoin1(uid,bonuesInfo.bonues,
                        function ( err )
                            
                            if err then
                                Debug.LogError(err);
                                local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                                { 
                                    status = responce.SystemError
                                }};
                                Session.SendPackage(s,retMsg);    
                                return;                    
                            end

                            local retMsg = {sType.System,cmdType.eRecvLoginBonuesRes,uid,
                            { 
                                status = responce.OK,
                            }};
                            Session.SendPackage(s,retMsg);    
                            return;   
                        end);
            end);
        end);
end

mysql.Connect();

return{
	GetUgameInfo=get_ugame_info,
    RecvLoginBonues=recv_login_bonues,
};