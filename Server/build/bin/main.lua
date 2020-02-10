log_debug("hello world")
mysql_wrapper.connect("127.0.0.1",3306,"test_mysql","root","Zzl5201314...",
function(err,context)
    log_debug("event call")
    mysql_wrapper.close()
end)