local mysql = require("database/mysql_center");
local redis = require("database/redis_center");
local sType = require("ServiceType");
local cmdType = require("auth/Const/CmdType");
local responce = require("Respones");

local function DoEditProfile( s,req )
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

local function user_login( s,req )

    local utag = req[3];
    local preq = req[4];

    local uname = preq.uname;
    local pwd = preq.pwd;
    --检验参数合法性
    if string.len(uname) <=0 then
        print("upgrade failed: InvalidParams",uname,pwd);
        local res = {sType.Auth,cmdType.UserLoginRes,utag,
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
                local retMsg = {sType.Auth,cmdType.UserLoginRes,utag,
                    { 
                        status = responce.SystemError
                    }};
                Session.SendPackage(s,retMsg);
                return;
            end
            --没有查到对应信息
            if uinfo==nil then
                print("UnameOrPwdError");
                local retMsg = {sType.Auth,cmdType.UserLoginRes,utag,
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
                local retMsg = {sType.Auth,cmdType.UserLoginRes,utag,
                    { 
                        status = responce.UserIsFreeze
                    }};
                Session.SendPackage(s,retMsg);
                return;
            end

            --登陆成功
            print(uinfo.uid.." "..uinfo.unick.." login");
            
            -- --将用户数据保存到Redis
            -- redis.SetUinfo(uinfo.uid,uinfo);

            local retMsg = {sType.Auth,cmdType.UserLoginRes,utag,
                { 
                    status = responce.OK,
                    uinfo = uinfo,
                }};
            --print("send utag: ",utag);
            Session.SendPackage(s,retMsg);
        end);
end

local function user_unregister( s,req )
    local uid = req[3];

    local res = {sType.Auth,cmdType.UserUnregisterRes,uid,
    {
        status = responce.OK,
    }};
    Session.SendPackage(s,res);
end

local function user_lost_conn(s, req)
    Debug.Log("a user disconnect from auth");
end

mysql.Connect();
redis.Connect();

return {
    UserLogin = user_login,
    UserUnregister=user_unregister,
    EditProfile=DoEditProfile,
    UserLostConn=user_lost_conn,
};