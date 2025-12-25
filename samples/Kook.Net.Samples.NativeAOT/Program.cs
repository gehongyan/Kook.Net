using Kook;
using Kook.WebSocket;

// Kook.Net NativeAOT 示例
// 此示例演示如何在 NativeAOT 编译模式下使用 Kook.Net

Console.WriteLine("Kook.Net NativeAOT Sample");
Console.WriteLine("========================");
Console.WriteLine();

// 从环境变量或配置文件读取 Token
string? token = Environment.GetEnvironmentVariable("KOOK_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Error: KOOK_TOKEN environment variable is not set.");
    Console.WriteLine("Please set your bot token:");
    Console.WriteLine("  export KOOK_TOKEN=\"your-bot-token-here\"");
    return 1;
}

// 创建客户端配置
// 注意：NativeAOT 不支持 Kook.Net.Commands 框架，因为它依赖反射
KookSocketConfig config = new()
{
    AlwaysDownloadUsers = false,
    MessageCacheSize = 100,
    LogLevel = LogSeverity.Info,
    StartupCacheFetchMode = StartupCacheFetchMode.Synchronous
};

// 创建客户端
using KookSocketClient client = new(config);

// 设置事件处理器
client.Log += LogAsync;
client.Ready += ReadyAsync;
client.MessageReceived += MessageReceivedAsync;

// 登录并启动
try
{
    await client.LoginAsync(TokenType.Bot, token);
    await client.StartAsync();

    Console.WriteLine("Bot is running. Press Ctrl+C to exit.");

    // 保持程序运行
    await Task.Delay(Timeout.Infinite);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}

return 0;

static Task LogAsync(LogMessage msg)
{
    Console.WriteLine($"[{msg.Severity}] {msg.Source}: {msg.Message}");
    if (msg.Exception != null)
        Console.WriteLine($"  Exception: {msg.Exception}");
    return Task.CompletedTask;
}

static Task ReadyAsync()
{
    Console.WriteLine("Bot is ready!");
    return Task.CompletedTask;
}

static Task MessageReceivedAsync(SocketMessage message, SocketGuildUser user, SocketTextChannel channel)
{
    // 忽略系统消息和 Bot 自己的消息
    if (message.Author.IsBot == true || message.Author.IsSystemUser)
        return Task.CompletedTask;

    // 简单的 ping 命令
    if (message.Content.Equals("!ping", StringComparison.OrdinalIgnoreCase))
    {
        // 在 NativeAOT 模式下，直接回复消息
        _ = message.Channel.SendTextAsync("Pong! 🏓");
    }

    return Task.CompletedTask;
}
