---
uid: Guides.QuickReference.Startup
title: 登录与启动
---

# 登录与启动

预声明变量

```csharp
readonly KookRestClient _restClient;
readonly KookSocketClient _socketClient;
```

### Rest 客户端

```csharp
// 使用默认配置创建 Rest 客户端
_restClient = new KookRestClient();
// 使用自定义配置创建 Rest 客户端
_restClient = new KookRestClient(new KookRestConfig()
{
    // 请求头 Accept-Language
    AcceptLanguage = "zh-CN",
    // 默认重试模式
    DefaultRetryMode = RetryMode.AlwaysRetry,
    // 默认超速回调
    DefaultRatelimitCallback = info => Task.CompletedTask,
    // 日志级别
    LogLevel = LogSeverity.Info,
    // 双向文稿格式化用户名
    FormatUsersInBidirectionalUnicode = true,
    // Rest 客户端提供程序
    RestClientProvider = DefaultRestClientProvider.Instance
});

// Token
string token = null;
// 登录
await _restClient.LoginAsync(TokenType.Bot, token);
// 登出
await _restClient.LogoutAsync();
```

### WebSocket 客户端

```csharp
// 使用默认配置创建 WebSocket 客户端
_socketClient = new KookRestClient();
// 使用自定义配置创建 WebSocket 客户端
_socketClient = new KookSocketClient(new KookRestConfig()
{
    // 请求头 Accept-Language
    AcceptLanguage = "zh-CN",
    // 默认重试模式
    DefaultRetryMode = RetryMode.AlwaysRetry,
    // 默认超速回调
    DefaultRatelimitCallback = info => Task.CompletedTask,
    // 日志级别
    LogLevel = LogSeverity.Info,
    // 双向文稿格式化用户名
    FormatUsersInBidirectionalUnicode = true,
    // Rest 客户端提供程序
    RestClientProvider = DefaultRestClientProvider.Instance,
    // 显示指定网关地址
    GatewayHost = null,
    // 连接超时（毫秒）
    ConnectionTimeout = 6000,
    // 处理程序警告耗时阈值（毫秒）
    HandlerTimeout = 3000,
    // 消息缓存数量
    MessageCacheSize = 10,
    // WebSocket 客户端提供程序
    WebSocketProvider = DefaultWebSocketProvider.Instance,
    // UDP 客户端提供程序
    UdpSocketProvider = DefaultUdpSocketProvider.Instance,
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
// 登出
await _socketClient.LogoutAsync();
```
