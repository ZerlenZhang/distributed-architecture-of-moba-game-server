# 分布式服务器架构下的环保主题3v3团队对抗游戏

【Warning】 本仓库作为本人的毕设课题，主要是初学者的学习/实践，谨慎用于正式用途。

## 仓库内容

1. 一个异步多线程的服务器框架。
2. 使用Lua语言开发的几个服务器业务逻辑
3. 一套基于Unity的对话系统套件
4. 一个以类似喷射战士的3v3对抗为核心玩法的游戏

## 异步多线程的分布式服务器框架

服务器框架位于`ServerCore`目录

### 框架特点

1. 采用libuv处理各种各样的异步操作，文件io，tcp，udp，工作队列等
2. 支持Protobuf和Json的序列化和反序列化，Protobuf支持动态读取.proto文件获取类型信息
3. 底层支持Mysql和Redis数据库
4. Session和Service的解耦合设计
5. 内置Lua虚拟机，将系统接口导出LuaAPI，支持Lua和C++混合开发，[查看导出的LuaAPI](https://github.com/ZerlenZhang/distributed-architecture-of-moba-game-server/wiki/Lua%E6%8E%A5%E5%8F%A3%E6%8C%87%E5%BC%95)
6. 其他工具：日志、时间戳、计时器等

### 核心类关系图
![Image text](https://github.com/ZerlenZhang/distributed-architecture-of-moba-game-server/raw/master/Images/UML_1.png)

`CmdPackage` 在网络层之上的一层自定义协议，本项目所有网络消息都是这个类型的。serviceType标识此消息是属于哪一类服务，cmdType标识此消息具体含义

`Session` 是对Socket的封装，提供发送CmdPackage的接口，上层无需关系是Tcp链接还是Udp链接

`Service` 是对服务器功能的抽象，服务器可以通过注册Service来获得处理某一类CmdPackage的功能。

`Netbus` 是框架的最底层，直接接触链接的建立，网络消息的发送和接受，它处理了字节流和CmdPackage之间的互相转化

## 服务器业务逻辑
服务器业务逻辑代码位于`ServerApp`目录

### 服务器启动示意图
![Image text](https://github.com/ZerlenZhang/distributed-architecture-of-moba-game-server/raw/master/Images/bushu.png)

### 已实现的功能
1. 支持用户的注册、登录、匹配和退出匹配、英雄选择、帧同步的服务器逻辑
2. 其他值得一提的是：时间原因，项目中未使用Redis，实际上是可以正常使用的

## 基于Unity的对话系统套件
对话系统代码主要位于`PurificationPioneer\Assets\3rd\DialogSystem`目录

### 对话系统示意图
![Image text](https://github.com/ZerlenZhang/distributed-architecture-of-moba-game-server/raw/master/Images/DialogStructure.png)

`DialogAsset` 是一种ScriptableObject类型的资产，支持可视化的编辑对话内容. 对话内容支持：对话、旁白、选择、音乐音乐、简单屏幕效果、逻辑分支、变量控制、自定义消息等等

`DialogSystem` 一方面，它作为一种容器存放当前物体需要使用到的DialogAsset，一方面，也作为对话系统的引擎，提供将DialogAsset进行演出的接口

`DialogTrigger` 可以方便的设置如何开启对话、如何使用DialogSystem中的DialogAsset列表、在什么情况下触发器正常工作等。

此外，系统还有DialogVarAsset、DialogProgresss、DialogCharacter等工具方便使用

## 3v3团队对抗游戏客户端

游戏客户端参考喷射战士的玩法，多人比拼在一定时间内涂色面积大的一方胜出
![Image text](https://github.com/ZerlenZhang/distributed-architecture-of-moba-game-server/raw/master/Images/Client.png)

### 客户端目前实现的内容
1. 用户注册、登录、匹配、英雄选择、帧同步战斗、战斗结束的结算与返回
2. 制作了三个可用的角色，分别具有不同的涂色方式
3. 制作了英雄图鉴、故事模式
4. 支持PC端和安卓两个平台


## 分布式服务器快速上手
1、在MobaServer.exe 同目录下
2、创建Server.lua,添加如下代码
~~~
Debug.LogInit("ServerLogs","server",true);
Netbus.TcpListen(8900,
function(s)      
    
        local ip,port = Session.GetAddress(s);
   
        print("new client come ["..ip..":"..port.."]");
 
   end);
print("Server tcp listen at 8900");
~~~
3、创建ServerBat.bat,记事本打开并添加如下内容
~~~
【添加Lua搜索路径，Lua启动脚本相对路径（相对搜索路径）】
MobaServer.exe ./ Server.lua
~~~
4、创建Client.lua,添加如下代码
~~~
Debug.LogInit("ClientLogs","client",true);
Netbus.TcpConnect("127.0.0.1",8900,
		--链接成功的回调
        function(session)
            if session then
                print("succeed to connect to [127.0.0.1:8900]");
                
                return;
            end
            print("failed to connect to [127.0.0.1:8900]")
        end);
~~~
5、创建ClientBat.bat，记事本打开添加如下内容
~~~
MobaServer.exe ./ Client.lua
~~~
6、先运行ServerBat，在运行ClientBat

