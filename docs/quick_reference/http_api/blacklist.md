---
uid: Guides.QuickReference.Http.Blacklist
title: 黑名单相关接口
---

# 黑名单相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketGuild socketGuild = null;

RestGuild restGuild = null;

IGuild guild = null;
```

### [获取黑名单列表]

GET `/api/v3/blacklist/list`

```csharp
IUser user = null; // 用户
ulong userId = default; // 用户 ID

// API 请求
IReadOnlyCollection<RestBan> restBansFromSocket = await socketGuild.GetBansAsync();
IReadOnlyCollection<RestBan> restBansFromRest = await restGuild.GetBansAsync();
IReadOnlyCollection<IBan> bans = await guild.GetBansAsync();

// API 请求，获取指定用户的封禁信息
IReadOnlyCollection<IBan> bans = await guild.GetBansAsync();
RestBan ban = await socketGuild.GetBanAsync(user);
RestBan ban = await socketGuild.GetBanAsync(userId);
RestBan ban = await restGuild.GetBanAsync(user);
RestBan ban = await restGuild.GetBanAsync(userId);
IBan ban = await guild.GetBanAsync(user);
IBan ban = await guild.GetBanAsync(userId);
```

### [加入黑名单]

POST `/api/v3/blacklist/create`

```csharp
IUser user = null; // 用户
ulong userId = default; // 用户 ID
int pruneDays = default; // 清理消息天数
string reason = null; // 理由

// API 请求
await guild.AddBanAsync(user, pruneDays, reason);
await guild.AddBanAsync(userId, pruneDays, reason);
```

### [移除黑名单]

POST `/api/v3/blacklist/delete`

```csharp
IUser user = null; // 用户
ulong userId = default; // 用户 ID

// API 请求
await guild.RemoveBanAsync(user);
await guild.RemoveBanAsync(userId);
```

[获取黑名单列表]: https://developer.kookapp.cn/doc/http/blacklist#获取黑名单列表
[加入黑名单]: https://developer.kookapp.cn/doc/http/blacklist#加入黑名单
[移除黑名单]: https://developer.kookapp.cn/doc/http/blacklist#移除黑名单
