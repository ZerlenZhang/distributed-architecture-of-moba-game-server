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

## 快速启动
1、写一个.bat脚本，放在MobaServer.exe同目录

~~~
【添加Lua搜索路径，Lua启动脚本相对路径（相对搜索路径）】
MobaServer.exe ./ Main.lua
~~~
2、然后在同目录创建Main.lua并加入以下代码
~~~
Debug.LogInit("logs","test",true);
print("hello from print");
Debug.Log("hello from Debug.Log");
~~~
3、运行bat脚本即可，看到“服务器退出"，不必惊讶，只要没有监听端口，就会直接退出


## 导出的LuaAPI
### 日志模块
~~~
//初始化日志模块，先初始化，使用才会有效
//【"日志目录相对路径","创建的文件前缀",是否打印到控制台】
eg:
Debug.LogInit("logger","gateway",true);
//日常使用
//【不定参数】
Debug.Log(arg1,arg2,...); // 等同于print
Debug.LogWarning(arg1,arg2,...);
Debug.LogError(arg1,arg2,...);
eg:
print("Gateway Server [tcp] listen at: ",gateway_tcp_port);
Debug.Log("Gateway Server [tcp] listen at: ",gateway_tcp_port);
~~~
### Protobuf模块
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
### 监听端口
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
### 内部的两种消息CmdPackage和RawPackage
0、数据结构：
~~~
    CmdPackage { short,short,int,char*}, 在lua中意义为{serviceType,cmdType,utag,body}
    RawPackage { short,short,int,char*,int}, 在lua中....你不会用到他的
~~~
1、使用 Session.SendRawPackage(session,rawpackage) 发送的包，接收CmdPackage的一方也可以收到，反之，使用Session.SendPackage(cmdPackage)发送的包，接收RawPackage的一方也可以收到，因为底层做了简单转化后在调用的Lua回调函数【这并不消耗性能】

2、RawPackage存在的意义是为了网关：网关有时需要知道包体的包头信息，但常规解析CmdPackage包会解析body，费事而无用，于是设计RawPackage供网关使用，
    所以，在搭建网关服务器，注册Service时，推荐使用Service.RegisterRaw而不是Service.Register，因为RawPackage提供一些方便的接口
~~~
serviceType,cmdType,utag = RawCmd.ReadHeader(rawpackage);//消耗很少，只读取前几个字节
body=RawCmd.ReadBody(rawpackage);   //这会把body解析出来，消耗大小视包体大小而异
RawCmd.SetUTag（rawpackage,value);
~~~
    
### Service模块
0、数据结构：
~~~
rawService数据结构：
{ 
    OnSessionRecvRaw=function(session,rawPackage)end,       // 必须
    OnSessionDisconnected=function(session,serviceType)end, // 必须
    OnSessionConnected=function(session,serviceType)end,    // 可选
}
service数据结构：
{ 
    OnSessionRecvCmd=function(session,cmdPackage)end,       // 必须
    OnSessionDisconnected=function(session,serviceType)end, // 必须
    OnSessionConnected=function(session,serviceType)end,    // 可选
});

//注册rawService【一般用于网关】
Service.RegisterRaw(serviceType,rawService);
///注册常规Service
Service.Register(serviceType,service);
~~~

