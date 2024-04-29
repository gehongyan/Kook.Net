using Kook;
using Kook.Commands;
using Kook.Net.Samples.CardMarkup.Services;
using Kook.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable SuggestVarOrType_BuiltInTypes

// 这是一个使用 Kook.Net 的 CardMarkup 扩展的示例
// 使用 CardMarkup 扩展，可以使用 XML 标记语言来构建卡片消息

// 设置 KookDebugToken 环境变量指定 Websocket Token 来运行此示例

// 示例文件 Cards/big-card.xml 包含了卡片消息中几乎所有元素
//  使用 !big-card 命令在 Kook 中发送此卡片消息

// 示例文件 Cards/vote.xml.liquid 使用 XML 定义卡片消息
//  使用 Liquid 模版引擎来在运行时定义卡片内容
//  使用 !vote 命令在 Kook 中发送此卡片消息

// 示例文件 Cards/multiple-cards.xml 在一个卡片消息中定义了多个卡片
//  使用 !multiple 命令在 Kook 中发送此卡片消息

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddInMemoryCollection(new[]
{
    new KeyValuePair<string, string?>("Logging:LogLevel:Default", "Debug")
});

builder.Services.AddSingleton<KookSocketConfig>(_ => new KookSocketConfig
{
    AlwaysDownloadUsers = false,
    AlwaysDownloadVoiceStates = false,
    AlwaysDownloadBoostSubscriptions = false,
    MessageCacheSize = 100,
    LogLevel = LogSeverity.Debug
});
builder.Services.AddSingleton<KookSocketClient>(sp =>
{
    KookSocketConfig config = sp.GetRequiredService<KookSocketConfig>();
    return new KookSocketClient(config);
});
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<CommandHandlerService>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddHostedService<KookSocketService>();

IHost app = builder.Build();

app.Run();
