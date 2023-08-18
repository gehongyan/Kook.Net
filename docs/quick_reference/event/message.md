---
uid: Guides.QuickReference.Event.Guild
title: 服务器相关事件
---

# 消息相关事件列表

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

本页结构与 KOOK 文档不完全一致，如需浏览 KOOK 文档，请参考：
* <https://developer.kookapp.cn/doc/event/message>

### 接收服务器频道消息

```csharp
_client.MessageReceived += (message, user, channel) => Task.CompletedTask;
```

### 接收私聊消息

```csharp
_client.DirectMessageReceived += (message, author, channel) => Task.CompletedTask;
```
