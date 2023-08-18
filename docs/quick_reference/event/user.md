---
uid: Guides.QuickReference.Event.User
title: 用户相关事件列表
---

# 用户相关事件列表

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

### [用户加入语音频道]

```csharp
_client.UserConnected += (user, channel, time) => Task.CompletedTask;
```

### [用户退出语音频道]

```csharp
_client.UserDisconnected += (user, channel, time) => Task.CompletedTask;
```

### [用户信息更新]

```csharp
_client.UserUpdated += (before, after) => Task.CompletedTask;
_client.CurrentUserUpdated += (before, after) => Task.CompletedTask;
```

### [自己新加入服务器]

```csharp
_client.JoinedGuild += guild => Task.CompletedTask;
```

### [自己退出服务器]

```csharp
_client.LeftGuild += guild => Task.CompletedTask;
```

### [Card 消息中的 Button 点击事件]

```csharp
_client.MessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;
_client.DirectMessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;
```

[用户加入语音频道]: https://developer.kookapp.cn/doc/event/user#用户加入语音频道
[用户退出语音频道]: https://developer.kookapp.cn/doc/event/user#用户退出语音频道
[用户信息更新]: https://developer.kookapp.cn/doc/event/user#用户信息更新
[自己新加入服务器]: https://developer.kookapp.cn/doc/event/user#自己新加入服务器
[自己退出服务器]: https://developer.kookapp.cn/doc/event/user#自己退出服务器
[Card 消息中的 Button 点击事件]: https://developer.kookapp.cn/doc/event/user#Card%20消息中的%20Button%20点击事件
