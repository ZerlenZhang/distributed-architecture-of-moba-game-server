# 学习高并发，高CPU利用率服务器架构

1. 异步网络链接模块，目前支持TCP，UDP，WebSocket协议
  1.1 抽象出Session，Service模块，便于拓展
  1.2 网络数据传输支持Json和Protobuf
2. 异步数据库操作模块，目前支持MySql,Redis
3. 异步持久化日志模块
4. 内置Lua虚拟机，上述模块全部具有Lua接口，可以纯Lua开发，也可Lua与C++混合开发
