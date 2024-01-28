using Kook;
using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TextCommandFramework.Services;

// 这是一个使用 Kook.Net 的文本命令框架的简单示例

// 此处使用了依赖注入，不需要实现 IDisposable 接口，using 声明会为我们处理弃置模式。
// 如果您使用另一个依赖注入框架，应该查阅其文档以找到最佳的处理方式。

// 您可以在以下位置找到使用命令框架的文档：
// - https://kooknet.dev/guides/text_commands/intro.html

await using ServiceProvider services = ConfigureServices();
KookSocketClient client = services.GetRequiredService<KookSocketClient>();

client.Log += LogAsync;
services.GetRequiredService<CommandService>().Log += LogAsync;

// 令牌（Tokens）应被视为机密数据，永远不应硬编码在代码中
// 在实际开发中，为了保护令牌的安全性，建议将令牌存储在安全的环境中
// 例如本地 .json、.yaml、.xml、.txt 文件、环境变量或密钥管理系统
// 这样可以避免将敏感信息直接暴露在代码中，以防止令牌被滥用或泄露
string token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User);
await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

// 此处初始化了命令所需的逻辑。
await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
await Task.Delay(Timeout.Infinite);
return;

// Log 事件，此处以直接输出到控制台为例
Task LogAsync(LogMessage log)
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
}

// 此处配置了依赖注入的服务
ServiceProvider ConfigureServices() => new ServiceCollection()
    .AddSingleton(_ => new KookSocketClient(new KookSocketConfig
    {
        AlwaysDownloadUsers = true,
        LogLevel = LogSeverity.Debug,
        AcceptLanguage = "en-US"
    }))
    .AddSingleton<CommandService>()
    .AddSingleton<CommandHandlingService>()
    .AddSingleton<HttpClient>()
    .AddSingleton<PictureService>()
    .BuildServiceProvider();
