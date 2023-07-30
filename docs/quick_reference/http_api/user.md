---
uid: Guides.QuickReference.Http.User
title: 用户相关接口
---

# 用户相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
IKookClient _client = null;
```

### [获取当前用户信息]

GET `/api/v3/user/me`

```csharp
IGuild guild = null; // 服务器

// 缓存获取当前用户信息
SocketSelfUser socketCurrentUser = _socketClient.CurrentUser;
RestSelfUser restCurrentUser = _restClient.CurrentUser;

// 缓存获取当前用户在服务器中的用户信息
SocketGuildUser socketGuildCurrentUser = _socketGuild.CurrentUser;

// API 请求
IUser currentUser = await _client.GetUserAsync(_client.CurrentUser.Id);
IGuildUser currentGuildUser = await guild.GetCurrentUserAsync();
```

### [获取目标用户信息]

GET `/api/v3/user/view`

```csharp
ulong userId = default; // 用户 ID
string username = null; // 用户名
string identifyNumber = null; // 用户标识码
IGuild guild = null; // 服务器

// 缓存获取目标用户信息
SocketUser socketUserById = _socketClient.GetUser(userId);
SocketUser socketUserByNameNumber = _socketClient.GetUser(username, identifyNumber);

// 缓存获取用户在服务器中的用户信息
SocketGuildUser socketGuildUser = socketGuild.GetUser(userId);

// API 请求
IUser user = await _client.GetUserAsync(userId);
IGuildUser user = await guild.GetUserAsync(userId)
```

### [下线机器人]

POST `/api/v3/user/offline`

```csharp
_socketClient.LogoutAsync();
_restClient.LogoutAsync();
```

[获取当前用户信息]: https://developer.kookapp.cn/doc/http/user#获取当前用户信息
[获取目标用户信息]: https://developer.kookapp.cn/doc/http/user#获取目标用户信息
[下线机器人]: https://developer.kookapp.cn/doc/http/user#下线机器人
