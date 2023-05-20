---
uid: Guides.QuickReference.Http.Channel
title: 频道相关接口列表
---

# 频道相关接口列表

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketGuild socketGuild = null;
SocketCategoryChannel socketCategoryChannel = null;

RestGuild restGuild = null;
IGuild guild = null;
IGuildUser guildUser = null;
```

### [获取频道列表]

GET `/api/v3/channel/list`

```csharp
// 缓存获取指定服务器的全部频道
IReadOnlyCollection<SocketGuildChannel> socketGuildChannels = socketGuild.Channels;
IReadOnlyCollection<RestGuildChannel> restGuildChannels = restGuild.Channels;
// 缓存获取指定服务器的全部文字频道
IReadOnlyCollection<SocketTextChannel> socketTextChannels = socketGuild.TextChannels;
IReadOnlyCollection<RestTextChannel> restTextChannels = restGuild.TextChannels;
// 缓存获取指定服务器的全部语音频道
IReadOnlyCollection<SocketVoiceChannel> socketVoiceChannels = socketGuild.VoiceChannels;
IReadOnlyCollection<RestVoiceChannel> restVoiceChannels = restGuild.VoiceChannels;
// 缓存获取指定服务器的全部分组频道
IReadOnlyCollection<SocketCategoryChannel> socketCategoryChannels = socketGuild.CategoryChannels;
IReadOnlyCollection<RestCategoryChannel> restCategoryChannels = restGuild.CategoryChannels;
// 缓存获取指定服务器分组频道下的全部频道
IReadOnlyCollection<SocketGuildChannel> socketChannelsInCategory = socketCategoryChannel.Channels;

// API 请求
IReadOnlyCollection<RestGuildChannel> restGuildChannels = await restGuild.GetChannelsAsync();
IReadOnlyCollection<RestTextChannel> restTextChannels = await restGuild.GetTextChannelsAsync();
IReadOnlyCollection<RestVoiceChannel> restVoiceChannels = await restGuild.GetVoiceChannelsAsync();
IReadOnlyCollection<RestCategoryChannel> restCategoryChannels = await restGuild.GetCategoryChannelsAsync();
```

[获取频道列表]: https://developer.kookapp.cn/doc/http/channel#获取频道列表
