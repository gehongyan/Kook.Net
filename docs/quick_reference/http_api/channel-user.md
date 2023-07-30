---
uid: Guides.QuickReference.Http.ChannelUser
title: 频道用户相关接口
---

# 频道用户相关

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [根据用户 ID 和服务器 ID 获取用户所在语音频道]

GET `/api/v3/channel-user/get-joined-channel`

```csharp
SocketGuildUser socketGuildUser = null;
IGuildUser guildUser = null;

// 要支持获取程序连接至 KOOK 网关前加入的语音频道信息，请设置 AlwaysDownloadVoiceStates = true
// 缓存获取用户所在语音频道
SocketVoiceChannel socketVoiceChannel = socketGuildUser.VoiceChannel;

// API 请求
IVoiceChannel voiceChannel = (await guildUser.GetConnectedVoiceChannelsAsync()).FirstOrDefault();
```

[根据用户 ID 和服务器 ID 获取用户所在语音频道]: https://developer.kookapp.cn/doc/http/channel-user#根据用户%20ID%20和服务器%20ID%20获取用户所在语音频道
