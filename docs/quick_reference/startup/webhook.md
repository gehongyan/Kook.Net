---
uid: Guides.QuickReference.Startup.Webhook
title: Webhook 客户端
---

# Webhook 客户端

Webhook 客户端的抽象类是 `KookWebhookClient`。

## ASP.NET 实现

```csharp
// 创建服务主机构建器
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 添加 KookAspNetWebhookClient 服务并进行必要的配置
builder.Services.AddKookAspNetWebhookClient(config =>
{
    // 包含 KookRestConfig 及 KookSocketConfig 的全部配置项，此处略

    // 由 KookWebhookConfig 提供的配置项
    // Webhook 负载验证令牌
    config.VerifyToken = default;
    // Webhook 负载解密密钥
    config.EncryptKey = default;
    // 启动时是否等待 Webhook 验证挑战后再开始启动 Bot 服务
    config.StartupWaitForChallenge = false;
    // Webhook 提供程序，此处为由 KookAspNetWebhookConfig 设置的默认值
    config.WebhookProvider = DefaultAspNetWebhookProvider.Instance;

    // 由 KookAspNetWebhookConfig 提供的配置项
    // 令牌类型
    config.TokenType = TokenType.Bot;
    // 令牌
    config.Token = default;
    // 是否验证令牌格式
    config.ValidateToken = true;
    // 请求的路由终结点
    config.RouteEndpoint = "kook";
    // 配置 KookAspNetWebhookClient
    config.ConfigureKookClient = (serviceProvider, client) => { };
});

// 构建服务主机
WebApplication app = builder.Build();

// 配置 Webhook 终结点
app.UseKookEndpoint();

// 启动服务主机
await app.RunAsync();
```

## HTTP Listener 实现

```csharp

// 使用默认配置创建 WebSocket 客户端
KookHttpListenerWebhookClient webhookClient = new KookHttpListenerWebhookClient();
// 使用自定义配置创建 WebSocket 客户端
KookHttpListenerWebhookClient webhookClient = new KookHttpListenerWebhookClient(new KookHttpListenerWebhookConfig
{
    // 包含 KookRestConfig 及 KookSocketConfig 的全部配置项，此处略

    // 由 KookWebhookConfig 提供的配置项
    // Webhook 负载验证令牌
    VerifyToken = default,
    // Webhook 负载解密密钥
    EncryptKey = default,
    // 启动时是否等待 Webhook 验证挑战后再开始启动 Bot 服务
    StartupWaitForChallenge = false,
    // Webhook 提供程序，此处为由 KookAspNetWebhookConfig 设置的默认值
    WebhookProvider = DefaultAspNetWebhookProvider.Instance,

    // 由 KookHttpListenerWebhookConfig 提供的配置项
    // 用于 HttpListener 的 URI 前缀列表
    UriPrefixes =
    [
        "http://localhost:5043/",
        "http://127.0.0.1:5043/"
    ],
    // HttpListener 崩溃后自动重启的时间间隔
    // Timeout.InfiniteTimeSpan 表示终止服务但不终止进程
    // 其它小于 0 的时间间隔表示不自动重启并终止进程
    // TimeSpan.Zero 表示立即重启 HttpListener
    // 其它大于 0 的时间间隔表示等待指定的时间间隔后重启 HttpListener
    AutoRestartInterval = TimeSpan.FromSeconds(5),

});

// Token
string token = null;
// 登录
await webhookClient.LoginAsync(TokenType.Bot, token);
// 启动
await webhookClient.StartAsync();
// 停止
await webhookClient.StopAsync();
// 登出
await webhookClient.LogoutAsync();
```
