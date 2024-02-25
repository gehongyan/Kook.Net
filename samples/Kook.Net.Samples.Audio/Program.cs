using Kook;
using Kook.Commands;
using Kook.Net.Samples.Audio.Modules;
using Kook.Net.Samples.Audio.Services;
using Kook.WebSocket;

// 这是一个使用 Kook.Net 的文本命令框架的简单示例

// 此处使用了依赖注入，不需要实现 IDisposable 接口，using 声明会为我们处理弃置模式。
// 如果您使用另一个依赖注入框架，应该查阅其文档以找到最佳的处理方式。

// 您可以在以下位置找到使用命令框架的文档：
// - https://kooknet.dev/guides/text_commands/intro.html

HostApplicationBuilder builder = new();
builder.Services.AddSingleton(_ => new KookSocketClient(new KookSocketConfig
{
    LogLevel = LogSeverity.Debug,
    MessageCacheSize = 100
}));
builder.Services.AddHostedService<KookClientService>();
builder.Services.AddSingleton(_ => new CommandService(new CommandServiceConfig
{
    DefaultRunMode = RunMode.Async
}));
builder.Services.AddSingleton<CommandHandlingService>();
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<CommandHandlingService>());
builder.Services.AddSingleton<MusicService>();
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<MusicService>());
builder.Services.AddHttpClient<MusicModule>("Music");

IHost app = builder.Build();
await app.RunAsync();
