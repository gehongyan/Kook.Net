---
uid: Guides.QuickReference.Startup.WebSocket
title: WebSocket 客户端
---

# WebSocket 客户端

预声明变量

```csharp
readonly KookSocketClient _socketClient;
```

```csharp
// 使用默认配置创建 WebSocket 客户端
_socketClient = new KookRestClient();
// 使用自定义配置创建 WebSocket 客户端
_socketClient = new KookSocketClient(new KookSocketConfig
{
    // 包含 KookRestConfig 的全部配置项，此处略

    // 显示指定网关地址
    GatewayHost = null,
    // 连接超时（毫秒）
    ConnectionTimeout = 6000,
    // 小型 Bot 服务器数量阈值
    SmallNumberOfGuildsThreshold = 5,
    // 大型 Bot 服务器数量阈值
    LargeNumberOfGuildsThreshold = 50,
    // 处理程序警告耗时阈值（毫秒）
    HandlerTimeout = 3000,
    // 消息缓存数量
    MessageCacheSize = 10,
    // WebSocket 客户端提供程序
    WebSocketProvider = DefaultWebSocketProvider.Instance,
    // UDP 客户端提供程序
    UdpSocketProvider = DefaultUdpSocketProvider.Instance,
    // 启动缓存数据获取模式
    StartupCacheFetchMode = StartupCacheFetchMode.Auto,
    // 自动下载服务器用户信息
    AlwaysDownloadUsers = false,
    // 自动下载服务器用户语音状态信息
    AlwaysDownloadVoiceStates = false,
    // 自动下载服务器助力信息
    AlwaysDownloadBoostSubscriptions = false,
    // 等待服务器可用状态超时（毫秒）
    MaxWaitBetweenGuildAvailablesBeforeReady = 10000,
    // 最大获取新加入服务器信息重试次数
    MaxJoinedGuildDataFetchingRetryTimes = 10,
    // 获取新加入服务器信息重试延迟（毫秒）
    JoinedGuildDataFetchingRetryDelay = 500
});

// Token
string token = null;
// 登录
await _socketClient.LoginAsync(TokenType.Bot, token);
// 启动
await _socketClient.StartAsync();
// 停止
await _socketClient.StopAsync();
// 登出
await _socketClient.LogoutAsync();
```
