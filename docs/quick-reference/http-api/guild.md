---
uid: Guides.QuickReference.Http.Guild
title: 服务器相关接口列表
---

# 服务器相关接口列表

### [获取当前用户加入的服务器列表]

GET `/api/v3/guild/list`

```csharp
// 缓存获取加入的所有服务器
IReadOnlyCollection<SocketGuild> socketGuilds = _socketClient.Guilds;
// API 请求
IReadOnlyCollection<RestGuild> restGuilds = await _socketClient.Rest.GetGuildsAsync();
IReadOnlyCollection<RestGuild> restGuilds = await _restClient.GetGuildsAsync();
```

### [获取服务器详情]

GET `/api/v3/guild/view`

```csharp
ulong guildId = 0; // 服务器 ID
// 缓存获取指定服务器
SocketGuild socketGuild = _socketClient.GetGuild(guildId);
// API 请求
RestGuild restGuild = await _socketClient.Rest.GetGuildAsync(guildId);
RestGuild restGuild = await _restClient.GetGuildAsync(guildId);
```

### [获取服务器中的用户列表]

GET `/api/v3/guild/user-list`

```csharp
ulong guildId = 0; // 服务器 ID
// 缓存获取 SocketGuild 对象
SocketGuild socketGuild = _socketClient.GetGuild(guildId);
// 缓存获取用户列表
IReadOnlyCollection<SocketGuildUser> socketUsers = socketGuild.Users;
// 调用异步方法获取用户列表，在缓存中包含所有用户的情况下，直接返回缓存的用户列表，否则会发起 API 请求获取分页结果
IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> pagedGuildUsers = socketGuild.GetUsersAsync();
// 合并分页结果
IEnumerable<IGuildUser> guildUsers = await pagedGuildUsers.FlattenAsync();

// 已有 RestGuild 对象的情况
RestGuild restGuild = null;
// API 请求
IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> pagedRestGuildUsers = restGuild.GetUsersAsync();
// 合并分页结果
IEnumerable<RestGuildUser> restGuildUsers = await pagedRestGuildUsers.FlattenAsync();

// 仅有 IGuild 对象的情况
IGuild guild = null;
// 如果 IGuild 对象是 SocketGuild，且缓存中包含所有用户，则直接返回缓存的用户列表，否则会发起 API 请求获取全部用户信息
IReadOnlyCollection<IGuildUser> guildUsers = await guild.GetUsersAsync();
```

### [修改服务器中用户的昵称]

POST `/api/v3/guild/nickname`

```csharp
string nickname = null; // 要设置为昵称，如要清空昵称，请传入 `null`、`string.Empty` 或该用户的 `IUser.Username`
// 已有 IGuildUser 对象
IGuildUser guildUser = null;
// API 请求
await guildUser.ModifyNicknameAsync(nickname);
// RestGuildUser 与 SocketGuildUser 均直接实现该方法
await restGuildUser.ModifyNicknameAsync(nickname);
await socketGuildUser.ModifyNicknameAsync(nickname);
```

### [离开服务器]

POST `/api/v3/guild/leave`

```csharp
// 已有 IGuild 对象
IGuild guild = null;
// API 请求
await guild.LeaveAsync();
// RestGuild 与 SocketGuild 均直接实现该方法
await restGuild.LeaveAsync();
await socketGuild.LeaveAsync();
```

### [踢出服务器]

POST `/api/v3/guild/kickout`

### [服务器静音闭麦列表]

GET `/api/v3/guild-mute/list`

### [添加服务器静音或闭麦]

POST `/api/v3/guild-mute/create`

### [删除服务器静音或闭麦]

POST `/api/v3/guild-mute/delete`

### [服务器助力历史]

GET `/api/v3/guild-boost/history`

[获取当前用户加入的服务器列表]: https://developer.kookapp.cn/doc/http/guild#获取当前用户加入的服务器列表
[获取服务器详情]: https://developer.kookapp.cn/doc/http/guild#获取服务器详情
[获取服务器中的用户列表]: https://developer.kookapp.cn/doc/http/guild#获取服务器中的用户列表
[修改服务器中用户的昵称]: https://developer.kookapp.cn/doc/http/guild#修改服务器中用户的昵称
[离开服务器]: https://developer.kookapp.cn/doc/http/guild#离开服务器
[踢出服务器]: https://developer.kookapp.cn/doc/http/guild#踢出服务器
[服务器静音闭麦列表]: https://developer.kookapp.cn/doc/http/guild#服务器静音闭麦列表
[添加服务器静音或闭麦]: https://developer.kookapp.cn/doc/http/guild#添加服务器静音或闭麦
[删除服务器静音或闭麦]: https://developer.kookapp.cn/doc/http/guild#删除服务器静音或闭麦
[服务器助力历史]: https://developer.kookapp.cn/doc/http/guild#服务器助力历史
