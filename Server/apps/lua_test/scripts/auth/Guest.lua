local mysql = require("datebase/mysql_center");
local redis = require("datebase/redis_center");
local sType = require("ServiceType");
local cmdType = require("auth/Const/CmdType");
local responce = require("Respones");
function OnGuestLogin( s,msg )
		--{sType,cType,utag,body}
    	local key = msg[4].guest_key;
        local utag = msg[3];

        --判断gkey的合法性
        if type(key) ~="string" or string.len(key) ~= 15 then
            print("invalid guest_key", err,key)
            local retMsg = {sType.Auth,cmdType.eGuestLoginRes,utag,
                { 
                    status = responce.InvalidParams,
                }};
            Session.SendPackage(s,retMsg);
            return;
        end

    	mysql.GetGuestUinfo(key,function( err,uinfo )
    		if err then
    			--告诉客户端某个错误信息
    			print("something wrong", err)
    			local retMsg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    				{ 
    					status = responce.SystemError
    				}};
    			Session.SendPackage(s,retMsg);
    			return;
    		end
    		--没有查到对应信息
    		if uinfo==nil then
    			--就添加进数据库
    			mysql.InsertGuestUser(key,function(err)
    				if err then
    					--告诉客户端某个错误信息
    					print("Insert err",err);
    					local retMsg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    						{ 
    							status = responce.SystemError
    						}};
    					Session.SendPackage(s,retMsg);
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
    			local retMsg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    				{ 
    					status = responce.UserIsFreeze
    				}};
    			Session.SendPackage(s,retMsg);
    			return;
    		end

            --账号已经不是游客账号
    		if uinfo.is_guest~=1 then
    			print("this is not a guest");
    			local retMsg = {sType.Auth,cmdType.eGuestLoginRes,utag,
    				{ 
    					status = responce.UserIsNotGuest
    				}};
    			Session.SendPackage(s,retMsg);
    			return;
    		end

    		--登陆成功
    		print(uinfo.uid.." "..uinfo.unick.." login");
    		
            --将用户数据保存到Redis
            redis.SetUinfo(uinfo.uid,uinfo);

            local retMsg = {sType.Auth,cmdType.eGuestLoginRes,utag,
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
            --print("send utag: ",utag);
    		Session.SendPackage(s,retMsg);
    	end);
end

function DoEditProfile( s,req )
    local uid = req[3];
    local editProfileReq = req[4];
    print(uid,editProfileReq.unick,editProfileReq.uface,editProfileReq.usex);

    if string.len(editProfileReq.unick)<=0 
        or (editProfileReq.uface<0 or editProfileReq.uface>7)
        or (editProfileReq.usex~=0 and editProfileReq.usex~=1) then
        local res = {sType.Auth,cmdType.eEditProfileRes,uid,
        {
            status = responce.InvalidParams,
        }};
        Session.SendPackage(s,res);
        return;
    end

    --更新数据库
    mysql.EditProfile(
        uid,editProfileReq.unick,editProfileReq.uface,editProfileReq.usex,
        function( err )
            local retStatus = responce.OK;
            if err then
                Debug.LogError(err);
                status = responce.SystemError;
            end

            --成功修改

            --修改Redis数据库
            redis.EditProfile(uid,editProfileReq.unick,editProfileReq.uface,editProfileReq.usex);

            local res = {sType.Auth,cmdType.eEditProfileRes,uid,
            {
                status = retStatus,
            }};
            Session.SendPackage(s,res);
        end);
    redis.EditProfile(uid,editProfileReq.unick,editProfileReq.uface,editProfileReq.usex);
end

mysql.Connect();
redis.Connect();

return {
	login = OnGuestLogin,
    edit_profile=DoEditProfile,
};