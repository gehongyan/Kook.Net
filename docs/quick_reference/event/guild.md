---
uid: Guides.QuickReference.Event.Guild
title: 服务器相关事件
---

# 服务器相关事件

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

### [服务器信息更新]

```csharp
_client.GuildUpdated += (before, after) => Task.CompletedTask;
```

### [服务器删除]

视同 [自己退出服务器](../event/user.md#自己退出服务器)

```csharp
_client.LeftGuild += guild => Task.CompletedTask;
```

### [服务器封禁用户]

```csharp
_client.UserBanned += (users, @operator, guild, reason) => Task.CompletedTask;
```

### [服务器取消封禁用户]

```csharp
_client.UserUnbanned += (users, @operator, guild) => Task.CompletedTask;
```

### [服务器添加新表情]

```csharp
_client.EmoteCreated += (emote, guild) => Task.CompletedTask;
```

### [服务器删除表情]

```csharp
_client.EmoteDeleted += (emote, guild) => Task.CompletedTask;
```

### [服务器更新表情]

```csharp
_client.EmoteUpdated += (before, after, guild) => Task.CompletedTask;
```

[服务器信息更新]: https://developer.kookapp.cn/doc/event/guild#服务器信息更新
[服务器删除]: https://developer.kookapp.cn/doc/event/guild#服务器删除
[服务器封禁用户]: https://developer.kookapp.cn/doc/event/guild#服务器封禁用户
[服务器取消封禁用户]: https://developer.kookapp.cn/doc/event/guild#服务器取消封禁用户
[服务器添加新表情]: https://developer.kookapp.cn/doc/event/guild#服务器添加新表情
[服务器删除表情]: https://developer.kookapp.cn/doc/event/guild#服务器删除表情
[服务器更新表情]: https://developer.kookapp.cn/doc/event/guild#服务器更新表情
