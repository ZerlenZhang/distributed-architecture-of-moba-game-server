local mysql = require("datebase/mysql_auth_center");
local sType = require("ServiceType");
local cmdType = require("auth/Const/CmdType");
local responce = require("Respones");
function OnGuestLogin( s,msg )
		--{sType,cType,utag,body}
		local utag = msg[3];
    	local key = msg[4].guest_key;
    	mysql.GetGuestUinfo(key,function( err,uinfo )
    		if err then
    			--告诉客户端某个错误信息
    			print("something wrong", err)
    			local msg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    				{ 
    					status = responce.SystemError
    				}};
    			Session.SendPackage(s,msg);
    			return;
    		end
    		--没有查到对应信息
    		if uinfo==nil then
    			--就添加进数据库
    			mysql.InsertGuestUser(key,function(err)
    				if err then
    					--告诉客户端某个错误信息
    					print("Insert err",err);
    					local msg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    						{ 
    							status = responce.SystemError
    						}};
    					Session.SendPackage(s,msg);
    					return;
    				end
    				--重新登陆
    				OnGuestLogin(s,msg);
    				return;
    			end)
    			return;
    		end
    		--找到对应游戏数据
    		if uinfo.status~=0 then
    			--游客账号被查封
    			print("this key has been frozen");
    			local msg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    				{ 
    					status = responce.UserIsFreeze
    				}};
    			Session.SendPackage(s,msg);
    			return;
    		end

    		if uinfo.is_guest~=1 then
    			--账号已经不是游客账号
    			print("this is not a guest");
    			local msg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    				{ 
    					status = responce.UserIsNotGuest
    				}};
    			Session.SendPackage(s,msg);
    			return;
    		end

    		--登陆成功
    		print(uinfo.uid.." "..uinfo.unick.." login");
    		local msg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    			{ 
    				status = responce.OK,
    				uinfo = {
    					unick=uinfo.unick,
    					uface=uinfo.uface,
    					usex=uinfo.usex,
    					uvip=uinfo.uvip,
    					uid=uinfo.uid,
    				},
    			}};
            print("send utag: ",utag);
    		Session.SendPackage(s,msg);
    	end);
end

return {
	login = OnGuestLogin,
};