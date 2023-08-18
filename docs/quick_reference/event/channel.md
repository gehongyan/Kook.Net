---
uid: Guides.QuickReference.Event.Guild
title: 服务器相关事件
---

# 频道相关事件

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

### [频道内用户添加 reaction]

```csharp
_client.ReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
```

### [频道内用户取消 reaction]

```csharp
_client.ReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;
```

### [频道消息更新]

```csharp
_client.MessageUpdated += (before, after, channel) => Task.CompletedTask;
```

### [频道消息被删除]

```csharp
_client.MessageDeleted += (message, channel) => Task.CompletedTask;
```

### [新增频道]

```csharp
_client.ChannelCreated += channel => Task.CompletedTask;
```

### [修改频道信息]

```csharp
_client.ChannelUpdated += (before, after) => Task.CompletedTask;
```

### [删除频道]

```csharp
_client.ChannelDestroyed += channel => Task.CompletedTask;
```

### [新的频道置顶消息]

```csharp
_client.MessagePinned += (before, after, channel, @operator) => Task.CompletedTask;
```

### [取消频道置顶消息]

```csharp
_client.MessageUnpinned += (before, after, channel, @operator) => Task.CompletedTask;
```

[频道内用户添加 reaction]: https://developer.kookapp.cn/doc/event/channel#频道内用户添加%20reaction
[频道内用户取消 reaction]: https://developer.kookapp.cn/doc/event/channel#频道内用户取消%20reaction
[频道消息更新]: https://developer.kookapp.cn/doc/event/channel#频道消息更新
[频道消息被删除]: https://developer.kookapp.cn/doc/event/channel#频道消息被删除
[新增频道]: https://developer.kookapp.cn/doc/event/channel#新增频道
[修改频道信息]: https://developer.kookapp.cn/doc/event/channel#修改频道信息
[删除频道]: https://developer.kookapp.cn/doc/event/channel#删除频道
[新的频道置顶消息]: https://developer.kookapp.cn/doc/event/channel#新的频道置顶消息
[取消频道置顶消息]: https://developer.kookapp.cn/doc/event/channel#取消频道置顶消息
