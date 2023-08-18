---
uid: Guides.QuickReference.Event.Guild
title: 服务器相关事件
---

# 服务器成员相关事件

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

### [新成员加入服务器]

```csharp
_client.UserJoined += (user, time) => Task.CompletedTask;
```

### [服务器成员退出]

```csharp
_client.UserLeft += (guild, user, time) => Task.CompletedTask;
```

### [服务器成员信息更新]

```csharp
_client.GuildMemberUpdated += (before, after) => Task.CompletedTask;
```

### [服务器成员上线]

```csharp
_client.GuildMemberOnline += (users, time) => Task.CompletedTask;
```

### [服务器成员下线]

```csharp
_client.GuildMemberOffline += (users, time) => Task.CompletedTask;
```

[新成员加入服务器]: https://developer.kookapp.cn/doc/event/guild-member#新成员加入服务器
[服务器成员退出]: https://developer.kookapp.cn/doc/event/guild-member#服务器成员退出
[服务器成员信息更新]: https://developer.kookapp.cn/doc/event/guild-member#服务器成员信息更新
[服务器成员上线]: https://developer.kookapp.cn/doc/event/guild-member#服务器成员上线
[服务器成员下线]: https://developer.kookapp.cn/doc/event/guild-member#服务器成员下线
