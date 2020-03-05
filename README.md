# 探索高并发，高CPU利用率的分布式服务器架构
![Image text](https://github.com/ZerlenZhang/Moba/blob/master/Images/architest.png)

采用C，C++开发底层，使用Lua写服务器逻辑，使用Unity制作客户端，结合MySql和Redis数据库，目前实现了登陆，编辑个人信息，游客账号升级，每日签到的逻辑，正在继续深入使用udp进一步开发帧同步框架

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


## 导出的LuaAPI
[详见Wiki](https://github.com/ZerlenZhang/distributed-architecture-of-moba-game-server/wiki/Lua%E6%8E%A5%E5%8F%A3%E6%8C%87%E5%BC%95)

