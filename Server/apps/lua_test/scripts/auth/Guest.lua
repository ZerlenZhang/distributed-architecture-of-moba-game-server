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

    	mysql.GetUinfoByKey(key,function( err,uinfo )
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
    --print(uid,editProfileReq.unick,editProfileReq.uface,editProfileReq.usex);

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

function _do_guest_account_upgrade(s,req,uid,uname,pwd )
    mysql.GuestAccountUpgrade(uid,uname,pwd,
        function ( err )
            --发生错误
            if err then
                local res = {sType.Auth,cmdType.eAccountUpgradeRes,uid,
                {
                    status = responce.SystemError,
                }};
                Session.SendPackage(s,res);
                return;
            end

            --更新成功 
            local res = {sType.Auth,cmdType.eAccountUpgradeRes,uid,
            {
                status = responce.OK,
            }};
            Session.SendPackage(s,res);
            print("[ "..uname.." : "..pwd.." ] upgrade success");
            return;
        end)
end

function account_upgrade( s,req )
    local uid = req[3];
    local preq = req[4];

    local uname = preq.uname;
    local pwd = preq.upwd_md5;

    --检验参数合法性
    if string.len(uname) <=0
        or string.len(pwd) ~= 32 then
        print("upgrade failed: InvalidParams",uname,pwd);
        local res = {sType.Auth,cmdType.eAccountUpgradeReq,uid,
        {
            status = responce.InvalidParams,
        }};
        Session.SendPackage(s,res);
        return;
    end

    --检查uid是否为游客
    mysql.CheckUnameExist(uname,
        function ( err, foundUname)
            --发生错误
            if err then
                Debug.LogError(err);
                local res = {sType.Auth,cmdType.eAccountUpgradeRes,uid,
                {
                    status = responce.SystemError,
                }};
                Session.SendPackage(s,res);
                return;
            end
            --uname已经被使用
            if foundUname then
                print("Upgrade failed: uname["..uname.."] has exist");
                local res = {sType.Auth,cmdType.eAccountUpgradeRes,uid,
                {
                    status = responce.UnameHasExist,
                }};
                Session.SendPackage(s,res);
                return;           
            end

            --检测是否已经不是游客账号
            mysql.GetUinfoByUid(uid,
                function ( err,uinfo )
                    if err then
                        Debug.LogError(err);
                        local res = {sType.Auth,cmdType.eAccountUpgradeRes,uid,
                        {
                            status = responce.SystemError,
                        }};
                        Session.SendPackage(s,res);
                        return;
                    end  
                    --如果已经不是游客账号
                    if uinfo.is_guest~=1 then
                        Debug.LogError("Upgrade failed: this is not a guest");
                        local res = {sType.Auth,cmdType.eAccountUpgradeRes,uid,
                        {
                            status = responce.UserIsNotGuest,
                        }};
                        Session.SendPackage(s,res);
                        return;
                    end

                    --可以升级
                    _do_guest_account_upgrade(s,req, uid,uname,pwd);
                end);

        end)
end

function user_login( s,req )

    local uid = req[3];
    local preq = req[4];

    local uname = preq.uname;
    local pwd = preq.upwd_md5;
    --检验参数合法性
    if string.len(uname) <=0
        or string.len(pwd) ~= 32 then
        print("upgrade failed: InvalidParams",uname,pwd);
        local res = {sType.Auth,cmdType.eUserLoginRes,uid,
        {
            status = responce.InvalidParams,
        }};
        Session.SendPackage(s,res);
        return;
    end

    --检查用户名与密码是否正确

    mysql.GetUinfoByUnamePwd(uname,pwd,function( err,uinfo )
            if err then
                --告诉客户端某个错误信息
                print("something wrong", err)
                local retMsg = {sType.Auth,cmdType.eUserLoginRes,uid,
                    { 
                        status = responce.SystemError
                    }};
                Session.SendPackage(s,retMsg);
                return;
            end
            --没有查到对应信息
            if uinfo==nil then
                print("UnameOrPwdError");
                local retMsg = {sType.Auth,cmdType.eUserLoginRes,uid,
                    { 
                        status = responce.UnameOrPwdError,
                    }};
                Session.SendPackage(s,retMsg);
                return;
            end
            --找到对应游戏数据
            if uinfo.status~=0 then
                --游客账号被查封
                print("this key has been frozen");
                local retMsg = {sType.Auth,cmdType.eUserLoginRes,uid,
                    { 
                        status = responce.UserIsFreeze
                    }};
                Session.SendPackage(s,retMsg);
                return;
            end

            --登陆成功
            print(uinfo.uid.." "..uinfo.unick.." login");
            
            --将用户数据保存到Redis
            redis.SetUinfo(uinfo.uid,uinfo);

            local retMsg = {sType.Auth,cmdType.eUserLoginRes,uid,
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

function user_unregister( s,req )
    local uid = req[3];

    local res = {sType.Auth,cmdType.eUserUnregisterRes,uid,
    {
        status = responce.OK,
    }};
    Session.SendPackage(s,res);
end

mysql.Connect();
redis.Connect();

return {
	GuestLogin = OnGuestLogin,
    UserLogin = user_login,
    UserUnregister=user_unregister,
    EditProfile=DoEditProfile,
    AccountUpgrade=account_upgrade,
};