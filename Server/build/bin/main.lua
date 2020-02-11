--打印table
function print_r ( t )  
    local print_r_cache={}
    local function sub_print_r(t,indent)
        if (print_r_cache[tostring(t)]) then
            print(indent.."*"..tostring(t))
        else
            print_r_cache[tostring(t)]=true
            if (type(t)=="table") then
                for pos,val in pairs(t) do
                    if (type(val)=="table") then
                        print(indent.."["..pos.."] => "..tostring(t).." {")
                        sub_print_r(val,indent..string.rep(" ",string.len(pos)+8))
                        print(indent..string.rep(" ",string.len(pos)+6).."}")
                    elseif (type(val)=="string") then
                        print(indent.."["..pos..'] => "'..val..'"')
                    else
                        print(indent.."["..pos.."] => "..tostring(val))
                    end
                end
            else
                print(indent..tostring(t))
            end
        end
    end
    if (type(t)=="table") then
        print(tostring(t).." {")
        sub_print_r(t,"  ")
        print("}")
    else
        sub_print_r(t,"  ")
    end
    print()
end

--测试service
local my_service={
    OnSessionRecvCmd=function(session,msg)
    end,
    OnSessionDisConnected=function(session)
    end
}

local ret = Service.Register(20,my_service)
Debug.Log("注册结果",ret)

--Mysql数据库
Mysql.Connect("127.0.0.1",3306,"test_mysql","root","Zzl5201314...",
function(err,context)
    Mysql.Query(context,"select * from test_class",
function(err,QueryTable)
    if err then
        Debug.LogError(err)
        return
    end
    Debug.Log("success")
    print_r(QueryTable)
    Mysql.Close(context)
end)
end)


--Redis数据库
Redis.Connect(
    "127.0.0.1",
    7999,
    function(error,context)
    
        if error then
            Debug.LogError(error)
        end

        Redis.Query(context,
        "hgetall 001",
        function(error,result)
            if error then
                Debug.LogError(error)
                return
            end
            print(result)
            Redis.Close(context)
        end)

    
    end);    

local timer = Timer.Repeat(
    function()
        print("timer");
    end,1000,-5,800);
print(timer)
Timer.Once(
    function()
        Timer.Cancel(timer);
        print("what ??????????");
    end,
    5000);


