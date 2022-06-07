---
uid: Guides.Concepts.ManageConnections
title: 管理连接
---

# 管理 KaiHeiLa.Net 中的连接

在 KaiHeiLa.Net 中，一旦客户端启动，除非客户端被手动关闭，
否则其与开黑啦网关之间将会尝试一直保持连接。

## 用法

要启动连接，请在客户端上调用 `StartAsync` 方法，这会启动一个 WebSocket 连接。
要终止连接，请在客户端上调用 `StopAsync` 方法，这会关闭任何已经打开的 WebSocket 连接。

由于启动/终止方法仅向底层连接管理器发送信号，指示其启动/终止连接，
启动/终止连接的操作是异步执行的，因此**这些方法会在启动/终止操作真正被执行前返回**。

因此，您需要订阅基于连接状态的事件，来准确地了解客户端何时启动/终止了连接。

所有的客户端都提供了 `Connected` 和 `Disconnected` 事件，
分别在连接启动或关闭时触发。需要注意的是，在 [KaiHeiLaSocketClient] 中，
`Connected` 并不代表客户端完成了初始化以供业务逻辑进行调用。

[KaiHeiLaSocketClient] 上提供了一个单独的事件 `Ready`，
仅当客户端下载完成所有必要的数据（如：服务器频道信息等），且拥有了完整了数据缓存，
该事件才会被触发。

[KaiHeiLaSocketClient]: xref:KaiHeiLa.WebSocket.KaiHeiLaSocketClient

## 重连

> [!TIP]
> 避免在网关现成上运行耗时代码！如果网关发生了如事件章节中所描述的死锁，
> 连接管理器将无法恢复并重新连接。

假设客户端由于网络波动、开黑啦服务端的重连请求或错误导致客户端断开连接，
而不是业务逻辑代码造成的死锁，客户端将会一直尝试重连并继续之前的会话。

不必担心如何维护连接，连接管理器的设计保证了重连机制的正常运行。
如果您的客户端没能成功地重连，或许这是一个 Bug，快来 [开黑啦 KaiHeiLa.Net 社区] 找我反馈吧！

[events]: xref:Guides.Concepts.Events
[开黑啦 KaiHeiLa.Net 社区]: https://kaihei.co/EvxnOb