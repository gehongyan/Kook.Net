---
uid: Guides.QuickReference.Event.Guild
title: 服务器相关事件
---

# 私聊消息事件

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

### [私聊消息更新]

```csharp
_client.DirectMessageUpdated += (before, after, author, channel) => Task.CompletedTask;
```

### [私聊消息被删除]

```csharp
_client.DirectMessageDeleted += (message, author, channel) => Task.CompletedTask;
```

### [私聊内用户添加 reaction]

```csharp
_client.DirectReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
```

### [私聊内用户取消 reaction]

```csharp
_client.DirectReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;
```

[私聊消息更新]: https://developer.kookapp.cn/doc/event/direct-message#私聊消息更新
[私聊消息被删除]: https://developer.kookapp.cn/doc/event/direct-message#私聊消息被删除
[私聊内用户添加 reaction]: https://developer.kookapp.cn/doc/event/direct-message#私聊内用户添加%20reaction
[私聊内用户取消 reaction]: https://developer.kookapp.cn/doc/event/direct-message#私聊内用户取消%20reaction
