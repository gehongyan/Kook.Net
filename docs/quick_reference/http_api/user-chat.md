---
uid: Guides.QuickReference.Http.UserChat
title: 私信聊天会话接口
---

# 私信聊天会话接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [获取私信聊天会话列表]

GET `/api/v3/user-chat/list`

```csharp
// 缓存获取私信聊天会话列表
IReadOnlyCollection<SocketDMChannel> cachedDmChannels = _client.DMChannels;

// API 请求
IReadOnlyCollection<IDMChannel> dmChannels = await _client.GetDMChannelsAsync();
```

### [获取私信聊天会话详情]

GET `/api/v3/user-chat/view`

```csharp
Guid chatCode = default; // 要获取详情的私信聊天会话的 ChatCode
ulong userId = default; // 要获取详情的私信聊天会话的 UserId

// 缓存获取私信聊天会话详情
SocketDMChannel dmChannelByChatCode = _client.GetDMChannel(chatCode);
SocketDMChannel dmChannelByUserId = _client.GetDMChannel(userId);

// API 请求
IDMChannel dmChannel = await _client.GetDMChannelAsync(chatCode);
```

### [创建私信聊天会话]

POST `/api/v3/user-chat/create`

```csharp
IUser user = null; // 要创建私信聊天会话的用户

// API 请求
IDMChannel dmChannel = await user.CreateDMChannelAsync();
```

### [删除私信聊天会话]

POST `/api/v3/user-chat/delete`

```csharp
IDMChannel dmChannel = null; // 要删除的私信聊天会话

// API 请求
await dmChannel.CloseAsync()
```

[获取私信聊天会话列表]: https://developer.kookapp.cn/doc/http/channel-user#获取私信聊天会话列表
[获取私信聊天会话详情]: https://developer.kookapp.cn/doc/http/channel-user#获取私信聊天会话详情
[创建私信聊天会话]: https://developer.kookapp.cn/doc/http/channel-user#创建私信聊天会话
[删除私信聊天会话]: https://developer.kookapp.cn/doc/http/channel-user#删除私信聊天会话
