---
uid: Guides.QuickReference.Http.Game
title: 用户动态相关接口
---

# 用户动态相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [游戏列表]

GET `/api/v3/game`

```csharp
GameCreationSource source = default; // 游戏创建来源

// API 请求
IAsyncEnumerable<IReadOnlyCollection<RestGame>> pagedGames = _socketClient.Rest.GetGamesAsync(source);
IAsyncEnumerable<IReadOnlyCollection<RestGame>> pagedGames = _restClient.GetGamesAsync(source);
```

### [添加游戏]

POST `/api/v3/game/create`

```csharp
string name = null; // 游戏名称
string processName = null; // 游戏进程名称
string iconUrl = null; // 图标地址

// API 请求
RestGame game = await _socketClient.Rest.CreateGameAsync(name, processName, iconUrl);
RestGame game = await _restClient.CreateGameAsync(name, processName, iconUrl);
```

### [更新游戏]

POST `/api/v3/game/update`

```csharp
RestGame game = null; // 游戏
string name = null; // 游戏名称
string iconUrl = null; // 图标地址

// API 请求
RestGame modifiedGame = await game.ModifyAsync(x =>
{
    x.Name = name;
    x.IconUrl = iconUrl;
});
```

### [删除游戏]

POST `/api/v3/game/delete`

```csharp
RestGame game = null; // 游戏

// API 请求
await game.DeleteAsync();
```

### [添加游戏/音乐记录(开始玩/听)]

POST `/api/v3/game/activity`

```csharp
RestGame game = null; // 游戏
Music music = null; // 音乐

// API 请求
await _socketClient.Rest.CurrentUser.StartPlayingAsync(game);
await _socketClient.Rest.CurrentUser.StartPlayingAsync(music);
await _restClient.CurrentUser.StartPlayingAsync(game);
await _restClient.CurrentUser.StartPlayingAsync(music);
```

### [删除游戏/音乐记录(结束玩/听)]

POST `/api/v3/game/delete-activity`

```csharp
ActivityType type = default; // 活动类型

// API 请求
await _socketClient.Rest.CurrentUser.StopPlayingAsync(type);
await _restClient.CurrentUser.StopPlayingAsync(type);
```

[游戏列表]: https://developer.kookapp.cn/doc/http/game#游戏列表
[添加游戏]: https://developer.kookapp.cn/doc/http/game#添加游戏
[更新游戏]: https://developer.kookapp.cn/doc/http/game#更新游戏
[删除游戏]: https://developer.kookapp.cn/doc/http/game#删除游戏
[添加游戏/音乐记录(开始玩/听)]: https://developer.kookapp.cn/doc/http/game#添加游戏/音乐记录-开始玩/听
[删除游戏/音乐记录(结束玩/听)]: https://developer.kookapp.cn/doc/http/game#删除游戏/音乐记录-结束玩/听
