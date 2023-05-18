---
uid: Guides.QuickReference.Http.Guild
title: 服务器相关接口列表
---

# 服务器相关接口列表

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketGuild socketGuild = null;
RestGuild restGuild = null;
IGuild guild = null;
IGuildUser guildUser = null;
```

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
// 要在启动时缓存服务器用户列表，请设置 AlwaysDownloadUsers = true
// 主动更新所有服务器用户列表缓存
await _socketClient.DownloadUsersAsync();
// 主动更新部分服务器用户列表缓存
IEnumerable<IGuild> guilds = Enumerable.Empty<IGuild>();
await _socketClient.DownloadUsersAsync(guilds);
// 主动更新指定服务器用户列表缓存
await socketGuild.DownloadUsersAsync();

ulong guildId = 0; // 服务器 ID
// 缓存获取 SocketGuild 对象
SocketGuild socketGuild = _socketClient.GetGuild(guildId);
// 缓存获取用户列表
IReadOnlyCollection<SocketGuildUser> socketUsers = socketGuild.Users;
// 调用异步方法获取用户列表，在缓存中包含所有用户的情况下，直接返回缓存的用户列表，否则会发起 API 请求获取分页结果
IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> pagedGuildUsers = socketGuild.GetUsersAsync();
// 合并分页结果
IEnumerable<IGuildUser> guildUsers = await pagedGuildUsers.FlattenAsync();

// 已有 RestGuild 对象
RestGuild restGuild = null;
// API 请求
IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> pagedRestGuildUsers = restGuild.GetUsersAsync();
// 合并分页结果
IEnumerable<RestGuildUser> restGuildUsers = await pagedRestGuildUsers.FlattenAsync();

// 如果 IGuild 对象是 SocketGuild，且缓存中包含所有用户，则直接返回缓存的用户列表，否则会发起 API 请求获取全部用户信息
IReadOnlyCollection<IGuildUser> guildUsers = await guild.GetUsersAsync();
```

### [修改服务器中用户的昵称]

POST `/api/v3/guild/nickname`

```csharp
// 要设置的昵称，如要清空昵称，请传入 `null`、`string.Empty` 或该用户的 `IUser.Username`
string nickname = null;
// API 请求
await guildUser.ModifyNicknameAsync(nickname);
```

### [离开服务器]

POST `/api/v3/guild/leave`

```csharp
// API 请求
await guild.LeaveAsync();
```

### [踢出服务器]

POST `/api/v3/guild/kickout`

```csharp
// API 请求
await guildUser.KickAsync();
```

### [服务器静音闭麦列表]

GET `/api/v3/guild-mute/list`

```csharp
// 要在启动时缓存语音状态信息，请设置 AlwaysDownloadVoiceStates = true
// 主动更新所有服务器语音状态信息缓存
await _socketClient.DownloadVoiceStatesAsync();
// 主动更新部分服务器语音状态信息缓存
IEnumerable<IGuild> guilds = Enumerable.Empty<IGuild>();
await _socketClient.DownloadVoiceStatesAsync(guilds);
// 主动更新指定服务器语音状态信息缓存
await socketGuild.DownloadVoiceStatesAsync();

// 缓存获取服务器内的所有语音状态信息
IEnumerable<SocketVoiceState?> guildUserVoiceStates = socketGuild.Users.Select(x => x.VoiceState);
// 缓存获取被服务器闭麦的用户列表
IEnumerable<SocketGuildUser> mutedUsers = socketGuild.Users.Where(x => x.VoiceState?.IsMuted == true);
// 缓存获取被服务器静音的用户列表
IEnumerable<SocketGuildUser> deafenedUsers = socketGuild.Users.Where(x => x.VoiceState?.IsDeafened == true);
```

### [添加服务器静音或闭麦]

POST `/api/v3/guild-mute/create`

```csharp
// API 请求
await guildUser.MuteAsync();
await guildUser.DeafenAsync();
```

### [删除服务器静音或闭麦]

POST `/api/v3/guild-mute/delete`

```csharp
// API 请求
await guildUser.UnmuteAsync();
await guildUser.UndeafenAsync();
```

### [服务器助力历史]

GET `/api/v3/guild-boost/history`

```csharp
// 要在启动时缓存服务器助力信息，请设置 AlwaysDownloadBoostSubscriptions = true
// 主动更新所有服务器服务器助力信息缓存
await _socketClient.DownloadBoostSubscriptionsAsync();
// 主动更新部分服务器服务器助力信息缓存
IEnumerable<IGuild> guilds = Enumerable.Empty<IGuild>();
await _socketClient.DownloadBoostSubscriptionsAsync(guilds);
// 主动更新指定服务器服务器助力信息缓存
await socketGuild.DownloadBoostSubscriptionsAsync();

// 缓存获取服务器内的所有服务器助力信息
ImmutableDictionary<IUser,IReadOnlyCollection<BoostSubscriptionMetadata>> boostSubscriptions = socketGuild.BoostSubscriptions;
ImmutableDictionary<IUser,IReadOnlyCollection<BoostSubscriptionMetadata>> validBoostSubscriptions = socketGuild.ValidBoostSubscriptions;
// 缓存获取服务器用户的服务器服务器助力信息
IReadOnlyCollection<BoostSubscriptionMetadata> boostSubscriptions = socketGuildUser.BoostSubscriptions;

// API 请求，获取服务器内的助力信息
ImmutableDictionary<IUser,IReadOnlyCollection<BoostSubscriptionMetadata>> boostSubscriptions = await guild.GetBoostSubscriptionsAsync();
ImmutableDictionary<IUser,IReadOnlyCollection<BoostSubscriptionMetadata>> boostSubscriptions = await guild.GetActiveBoostSubscriptionsAsync();
// API 请求，获取服务器用户的助力信息
IReadOnlyCollection<BoostSubscriptionMetadata> boostSubscriptions = await guildUser.GetBoostSubscriptionsAsync();
```

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
