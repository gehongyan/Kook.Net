---
uid: Guides.QuickReference.Http.GuildEmoji
title: 服务器表情相关接口
---

# 服务器表情相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketGuild socketGuild = null;

RestGuild restGuild = null;

IGuild guild = null;
```

### [获取服务器表情列表]

GET `/api/v3/guild-emoji/list`

```csharp
string emoteId = null; // 表情符号 ID

// 缓存中获取表情符号列表
IReadOnlyCollection<GuildEmote> cachedGuildEmotes = guild.Emotes;
// 缓存获取指定 ID 的表情符号
GuildEmote cachedGuildEmote = socketGuild.GetEmote(emoteId);

// API 请求获取表情符号列表
IReadOnlyCollection<GuildEmote> guildEmotes = await guild.GetEmotesAsync();
GuildEmote guildEmote = await guild.GetEmoteAsync(emoteId);
```

### [创建服务器表情]

POST `/api/v3/guild-emoji/create`

```csharp
string name = null; // 表情符号名称
Image image = default; // 表情符号图片

// API 请求
GuildEmote emote = await guild.CreateEmoteAsync(name, image);
```

### [更新服务器表情]

POST `/api/v3/guild-emoji/update`

```csharp
GuildEmote emote = null; // 表情符号
string name = null; // 表情符号名称

// API 请求
await guild.ModifyEmoteNameAsync(emote, name);
```

### [删除服务器表情]

POST `/api/v3/guild-emoji/delete`

```csharp
GuildEmote emote = null; // 表情符号

// API 请求
await guild.DeleteEmoteAsync(emote);
```

[获取服务器表情列表]: https://developer.kookapp.cn/doc/http/guild-emoji#获取服务器表情列表
[创建服务器表情]: https://developer.kookapp.cn/doc/http/guild-emoji#创建服务器表情
[更新服务器表情]: https://developer.kookapp.cn/doc/http/guild-emoji#更新服务器表情
[删除服务器表情]: https://developer.kookapp.cn/doc/http/guild-emoji#删除服务器表情
