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

function RedisCb(error,result)
    if error then
        log_error(error)
        return
    end
    print(result)
end

table.print = print_r

log_debug("hello world")
mysql_wrapper.connect("127.0.0.1",3306,"test_mysql","root","Zzl5201314...",
function(err,context)
    mysql_wrapper.query(context,"select * from test_class",
function(err,queryTable)
    if err then
        log_error(err)
        return
    end
    log_debug("success")
    print_r(queryTable)
    mysql_wrapper.close(context)
end)
end)

redis_wrapper.connect(
    "127.0.0.1",
    7999,
    function(error,context)
    
        if error then
            log_error(error)
        end

        redis_wrapper.query(context,
        "hgetall 001",
        function(error,result)
            if error then
                log_error(error)
                return
            end
            print(result)
            redis_wrapper.close_redis(context)
        end)

    
    end);