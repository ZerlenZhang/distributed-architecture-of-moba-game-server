local mysql = require("database/mysql_center");
local sType = require("ServiceType");
local cmdType = require("auth/const/CmdType");
local responce = require("Respones");

local function on_user_registe(s,req)
    local utag=req[3];
    local info=req[4];
    local uname=info.uname;
    local pwd=info.pwd;
    local unick=info.unick;

    --检验参数合法性
    if string.len(uname) <=0 or string.len(pwd) <=0 or string.len(unick) <=0 then
        local res = {sType.Auth,cmdType.UserRegisteRes,utag,
        {
            status = responce.InvalidParams,
        }};
        Session.SendPackage(s,res);
        return;
    end

    mysql.CheckUnameExist(uname,function(err,uid)
        if err then
            Debug.LogError("Check uname err:"..err);
            local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
                { 
                    status = responce.SystemError
                }};
            Session.SendPackage(s,retMsg);
            return;
        end

        if uid then
            --重复注册
            local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
                { 
                    status = responce.UnameHasExist
                }};
            Session.SendPackage(s,retMsg);
            return;
        end

        mysql.Registe(uname,pwd,unick,function(err)
            if err then
                Debug.LogError("Registe error:"..err);
                local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
                    { 
                        status = responce.SystemError
                    }};
                Session.SendPackage(s,retMsg);
                return;
            end


            mysql.GetUinfoByUnamePwd(uname,pwd,function( err,uinfo )
                if err then
                    --告诉客户端某个错误信息
                    print("something wrong", err)
                    local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
                        { 
                            status = responce.SystemError
                        }};
                    Session.SendPackage(s,retMsg);
                    return;
                end
                --没有查到对应信息
                if uinfo==nil then
                    print("UnameOrPwdError");
                    local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
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
                    local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
                        { 
                            status = responce.UserIsFreeze
                        }};
                    Session.SendPackage(s,retMsg);
                    return;
                end

                --登陆成功
                print(uinfo.uid.." "..uinfo.unick.." registe and login".." uface-"..uinfo.uface.." heros-"..uinfo.heros);

                local retMsg = {sType.Auth,cmdType.UserRegisteRes,utag,
                    { 
                        status = responce.Ok,
                        uinfo = uinfo,
                    }};
                --print("send utag: ",utag);
                Session.SendPackage(s,retMsg);

            end);--mysql.GetUinfoByUnamePwd

        end);--mysql.Registe
    
    end);--mysql.CheckUnameExist
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
            print(uinfo.uid.." "..uinfo.unick.." login".." uface-"..uinfo.uface.." heros-"..uinfo.heros);
            
            local retMsg = {sType.Auth,cmdType.UserLoginRes,utag,
                { 
                    status = responce.Ok,
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
        status = responce.Ok,
    }};
    Session.SendPackage(s,res);
end

local function user_lost_conn(s, req)
    Debug.Log("a user disconnect from auth");
end

mysql.Connect();

return {
    UserLogin = user_login,
    UserUnregister=user_unregister,
    EditProfile=DoEditProfile,
    UserLostConn=user_lost_conn,
    OnUserRegiste=on_user_registe,
};