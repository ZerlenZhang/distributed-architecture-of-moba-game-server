# 学习高并发，高CPU利用率服务器架构

## 核心原理
1、采用libuv处理各种各样的异步操作，文件io，tcp，udp，工作队列，计时器等

2、Session和Service的解耦和设计

3、内置Lua虚拟机，并导出大量接口，支持纯Lua语言开发以及C++，Lua混合开发。

## 核心模块架构
![Image text](https://github.com/ZerlenZhang/Moba/blob/master/Images/UML_1.png)

## 其他功能
1、CmdPackage的body支持Json和Protobuf

2、内部支持链接MySql和Redis数据库

3、异步日志持久化

## 优化
1、大量使用对象池，避免内存碎片化

## 导出的LuaAPI
1、日志模块
~~~
//初始化日志模块，先初始化，使用才会有效
//【"相对路径","创建的文件前缀",是否打印到控制台】
eg:
Debug.LogInit("logger/gateway","gateway",true); //现在需要logger目录事先存在
//日常使用
//【不定参数】
Debug.Log(arg1,arg2,...); // 等同于print
Debug.LogWarning(arg1,arg2,...);
Debug.LogError(arg1,arg2,...);
eg:
print("Gateway Server [tcp] listen at: ",gateway_tcp_port);
Debug.Log("Gateway Server [tcp] listen at: ",gateway_tcp_port);
~~~
2、Protobuf模块
~~~
//初始化协议
local ProtoType={
    Json=0,
    Protobuf=1,
};
//如果是Protobuf，第二个参数是.proto文件所在目录，
//如果是Json，第二参数可以不传
ProtoManager.Init(ProtoType.Protobuf,protofiledir);
~~~
3、监听端口
~~~
//监听Tcp协议端口,第二参数可以不传
Netbus.TcpListen(port,
      function(session)
        local ip,port = Session.GetAddress(session);
        print("new client come ["..ip..":"..port.."]");
      end);
//监听websocket协议端口,第二参数可以不传
Netbus.WebsocketListen(port,
      function(session)
        local ip,port = Session.GetAddress(session);
        print("new client come ["..ip..":"..port.."]");
      end);
//监听Udp协议端口
Netbus.UdpListen(port);
~~~


