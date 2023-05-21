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
SocketVoiceChannel socketVoiceChannel = null;
SocketCategoryChannel socketCategoryChannel = null;

RestGuild restGuild = null;

IGuild guild = null;
IGuildUser guildUser = null;
IGuildChannel guildChannel = null;
ITextChannel textChannel = null;
IVoiceChannel voiceChannel = null;
INestedChannel nestedChannel = null;
ICategoryChannel categoryChannel = null;
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

// 如果 IGuild 对象是 SocketGuild，则直接返回缓存的频道列表，否则会发起 API 请求获取全部频道信息
IReadOnlyCollection<IGuildChannel> guildChannels = await guild.GetChannelsAsync();
IReadOnlyCollection<ITextChannel> textChannels = await guild.GetTextChannelAsyncs();
IReadOnlyCollection<IVoiceChannel> voiceChannels = await guild.GetVoiceChannelAsyncs();
IReadOnlyCollection<ICategoryChannel> categoryChannels = await guild.GetCategoryChannelAsyncs();
```

### [获取频道详情]

GET `/api/v3/channel/view`

```csharp
ulong channelId = default; // 频道 ID
// 缓存获取指定的服务器频道
SocketChannel socketChannel = _socketClient.GetChannel(channelId);
SocketChannel socketChannel = socketGuild.GetChannel(channelId);
RestGuildChannel restGuildChannel = restGuild.Channels.FirstOrDefault(x => x.Id == channelId);
// 缓存获取指定的服务器文字频道
SocketTextChannel socketTextChannel = socketGuild.GetTextChannel(channelId);
RestTextChannel restTextChannel = restGuild.TextChannels.FirstOrDefault(x => x.Id == channelId);
// 缓存获取指定的服务器语音频道
SocketVoiceChannel socketVoiceChannel = socketGuild.GetVoiceChannel(channelId);
RestVoiceChannel restVoiceChannel = restGuild.VoiceChannels.FirstOrDefault(x => x.Id == channelId);
// 缓存获取指定的服务器分组频道
SocketCategoryChannel socketCategoryChannel = socketGuild.GetCategoryChannel(channelId);
RestCategoryChannel restCategoryChannel = restGuild.CategoryChannels.FirstOrDefault(x => x.Id == channelId);

// API 请求
RestChannel restChannel = await _restClient.GetChannelAsync(channelId);
RestGuildChannel restGuildChannel = await restGuild.GetChannelAsync(channelId);
RestTextChannel restTestChannel = await restGuild.GetTextChannelAsync(channelId);
RestVoiceChannel restVoiceChannel = await restGuild.GetVoiceChannelAsync(channelId);
RestCategoryChannel restCategoryChannel = await restGuild.GetCategoryChannelAsync(channelId);

// 如果 IGuild 对象是 SocketGuild，则直接返回缓存的频道，否则会发起 API 请求获取指定的频道信息
IChannel channel = await _socketClient.GetChannelAsync(channelId);
IGuildChannel guildChannel = await guild.GetChannelAsync(channelId);
ITextChannel textChannel = await guild.GetTextChannelAsync(channelId);
IVoiceChannel voiceChannel = await guild.GetVoiceChannelAsync(channelId);
ICategoryChannel categoryChannel = await guild.GetCategoryChannelAsync(channelId);
```

### [创建频道]

POST `/api/v3/channel/create`

```csharp
string name = null; // 频道名称
ulong categoryChannelId = default; // 分组频道 ID
string topic = null; // 文字频道主题
int userLimit = default; // 语音频道用户数上限
VoiceQuality voiceQuality = default; // 语音频道语音质量

// API 请求，创建文字频道
ITextChannel textChannel = await guild.CreateTextChannelAsync(name);
ITextChannel textChannel = await guild.CreateTextChannelAsync(name, x =>
{
    x.CategoryId = categoryChannelId;
    x.Topic = topic;
});
// API 请求，创建语音频道
IVoiceChannel voiceChannel = await guild.CreateVoiceChannelAsync(name);
IVoiceChannel voiceChannel = await guild.CreateVoiceChannelAsync(name, x =>
{
    x.CategoryId = categoryChannelId;
    x.UserLimit = userLimit;
    x.VoiceQuality = voiceQuality;
});
// API 请求，创建分组频道
ICategoryChannel categoryChannel = await guild.CreateCategoryChannelAsync(name);
```

### [编辑频道]

POST `/api/v3/channel/update`

```csharp
string name = null; // 频道名称
ulong categoryChannelId = default; // 分组频道 ID
int position = default; // 频道排序位置
string topic = null; // 文字频道主题
SlowModeInterval slowModeInterval = default; // 文字频道慢速模式间隔
int userLimit = default; // 语音频道用户数上限
VoiceQuality voiceQuality = default; // 语音频道语音质量
string password = null; // 语音频道密码

// API 请求，修改频道
guildChannel.ModifyAsync(x =>
{
    x.Name = name;
    x.CategoryId = categoryChannelId;
    x.Position = position;
});
// API 请求，修改文字频道
textChannel.ModifyAsync(x =>
{
    x.Name = name;
    x.CategoryId = categoryChannelId;
    x.Position = position;
    x.Topic = topic;
    x.SlowModeInterval = slowModeInterval;
});
// API 请求，修改语音频道
voiceChannel.ModifyAsync(x =>
{
    x.Name = name;
    x.CategoryId = categoryChannelId;
    x.Position = position;
    x.VoiceQuality = voiceQuality;
    x.UserLimit = userLimit;
    x.Password = password;
});
// API 请求，修改分组频道
categoryChannel.ModifyAsync(x =>
{
    x.Name = name;
    x.CategoryId = categoryChannelId;
    x.Position = position;
});
```

### [删除频道]

POST `/api/v3/channel/delete`

```csharp
// API 请求
await guildChannel.DeleteAsync();
```

### [语音频道用户列表]

GET `/api/v3/channel/user-list`

```csharp
// 要在启动时缓存语音状态信息，请设置 AlwaysDownloadVoiceStates = true
// 主动更新所有服务器语音状态信息缓存
await _socketClient.DownloadVoiceStatesAsync();
// 缓存获取
IReadOnlyCollection<SocketGuildUser> connectedGuildUsers = socketVoiceChannel.ConnectedUsers;
// API 请求
IReadOnlyCollection<IUser> connectedUsers = await voiceChannel.GetConnectedUsersAsync();
```

### [语音频道之间移动用户]

POST `/api/v3/channel/move-user`

```csharp
// 要移动的服务器用户列表
IEnumerable<IGuildUser> guildUsers = null;
// API 请求
await guild.MoveUsersAsync(guildUsers, voiceChannel);
```

### [获取频道角色权限详情]

GET `/api/v3/channel-role/index`

```csharp
IRole role = null; // 要获取在该频道的权限覆盖配置的角色
IUser user = null; // 要获取在该频道的权限覆盖配置的用户

// 缓存获取频道的角色或用户权限覆盖配置
IReadOnlyCollection<RolePermissionOverwrite> rolePermissionOverwrites = guildChannel.RolePermissionOverwrites;
IReadOnlyCollection<UserPermissionOverwrite> userPermissionOverwrites = guildChannel.UserPermissionOverwrites;
// 缓存获取频道的指定角色或用户的权限覆盖配置
OverwritePermissions? rolePermissionOverwrite = guildChannel.GetPermissionOverwrite(role);
OverwritePermissions? userPermissionOverwrite = guildChannel.GetPermissionOverwrite(user);
```

### [创建频道角色权限]

POST `/api/v3/channel-role/create`

```csharp
IRole role = null; // 要在该频道创建权限覆盖配置的角色
IGuildUser guildUser = null; // 要在该频道创建权限覆盖配置的服务器用户

// API 请求
await guildChannel.AddPermissionOverwriteAsync(role);
await guildChannel.AddPermissionOverwriteAsync(guildUser);
```

### [更新频道角色权限]

POST `/api/v3/channel-role/update`

```csharp
IRole role = null; // 要在该频道修改权限覆盖配置的角色
IGuildUser guildUser = null; // 要在该频道修改权限覆盖配置的服务器用户

// API 请求
await guildChannel.ModifyPermissionOverwriteAsync(role, x => x.Modify());
await guildChannel.ModifyPermissionOverwriteAsync(role, x => x.Modify());
```

### [同步频道角色权限]

POST `/api/v3/channel-role/sync`

```csharp
// API 请求
await nestedChannel.SyncPermissionsAsync();
```

### [删除频道角色权限]

POST `/api/v3/channel-role/delete`

```csharp
// API 请求
await guildChannel.RemovePermissionOverwriteAsync(role);
await guildChannel.RemovePermissionOverwriteAsync(guildUser);
```

[获取频道列表]: https://developer.kookapp.cn/doc/http/channel#获取频道列表
[获取频道详情]: https://developer.kookapp.cn/doc/http/channel#获取频道详情
[创建频道]: https://developer.kookapp.cn/doc/http/channel#创建频道
[编辑频道]: https://developer.kookapp.cn/doc/http/channel#编辑频道
[删除频道]: https://developer.kookapp.cn/doc/http/channel#删除频道
[语音频道用户列表]: https://developer.kookapp.cn/doc/http/channel#语音频道用户列表
[语音频道之间移动用户]: https://developer.kookapp.cn/doc/http/channel#语音频道之间移动用户
[获取频道角色权限详情]: https://developer.kookapp.cn/doc/http/channel#频道角色权限详情
[创建频道角色权限]: https://developer.kookapp.cn/doc/http/channel#创建频道角色权限
[更新频道角色权限]: https://developer.kookapp.cn/doc/http/channel#更新频道角色权限
[同步频道角色权限]: https://developer.kookapp.cn/doc/http/channel#同步频道角色权限
[删除频道角色权限]: https://developer.kookapp.cn/doc/http/channel#删除频道角色权限
