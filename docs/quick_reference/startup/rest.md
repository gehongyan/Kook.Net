---
uid: Guides.QuickReference.Startup.Rest
title: Rest 客户端
---

# Rest 客户端

预声明变量

```csharp
readonly KookRestClient _restClient;
```

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
