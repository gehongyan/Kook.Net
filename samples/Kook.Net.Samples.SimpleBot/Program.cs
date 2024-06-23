using Kook;
using Kook.WebSocket;

// 这是一个使用 Kook.Net 的 C# 简单示例

// 如果要编写带有命令的机器人，我们建议使用 Kook.Net.Commands 框架
// 而不是像在这个示例中一样自己处理它们。

// 您可以在以下位置找到使用命令框架的示例：
// - 这里可以看基于文本的命令框架的指南与示例
// - https://kooknet.dev/guides/text_commands/intro.html

// KookSocketConfig 是 KookSocketClient 的配置类
KookSocketConfig config = new()
{
    AlwaysDownloadUsers = false,
    AlwaysDownloadVoiceStates = false,
    AlwaysDownloadBoostSubscriptions = false,
    MessageCacheSize = 100,
    LogLevel = LogSeverity.Debug,
    AutoUpdateChannelPositions = true,
    AutoUpdateRolePositions = true,
    StartupCacheFetchMode = StartupCacheFetchMode.Lazy,
    LargeNumberOfGuildsThreshold = 50
};

// 在使用完 Kook.Net 的客户端后，建议在应用程序的生命周期结束时进行 Dispose 操作
using KookSocketClient client = new(config);

// 此处列举了 Kook.Net 的 KookSocketClient 的所有事件

#region BaseKookClient

client.Log += LogAsync;
client.LoggedIn += () => Task.CompletedTask;
client.LoggedOut += () => Task.CompletedTask;

#endregion

#region KookSocketClient

client.Connected += () => Task.CompletedTask;
client.Disconnected += exception => Task.CompletedTask;
client.Ready += ReadyAsync;
client.LatencyUpdated += (before, after) => Task.CompletedTask;

#endregion

#region BaseSocketClient

client.ChannelCreated += channel => Task.CompletedTask;
client.ChannelDestroyed += channel => Task.CompletedTask;
client.ChannelUpdated += (before, after) => Task.CompletedTask;

client.ReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
client.ReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;
client.DirectReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
client.DirectReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;

client.MessageReceived += MessageReceivedAsync;
client.MessageDeleted += (message, channel) => Task.CompletedTask;
client.MessageUpdated += (before, after, channel) => Task.CompletedTask;
client.MessagePinned += (before, after, channel, @operator) => Task.CompletedTask;
client.MessageUnpinned += (before, after, channel, @operator) => Task.CompletedTask;

client.DirectMessageReceived += (message, author, channel) => Task.CompletedTask;
client.DirectMessageDeleted += (message, author, channel) => Task.CompletedTask;
client.DirectMessageUpdated += (before, after, author, channel) => Task.CompletedTask;

client.UserJoined += (user, time) => Task.CompletedTask;
client.UserLeft += (guild, user, time) => Task.CompletedTask;
client.UserBanned += (users, @operator, guild, reason) => Task.CompletedTask;
client.UserUnbanned += (users, @operator, guild) => Task.CompletedTask;
client.UserUpdated += (before, after) => Task.CompletedTask;
client.CurrentUserUpdated += (before, after) => Task.CompletedTask;
client.GuildMemberUpdated += (before, after) => Task.CompletedTask;
client.GuildMemberOnline += (users, time) => Task.CompletedTask;
client.GuildMemberOffline += (users, time) => Task.CompletedTask;
client.UserVoiceStateUpdated += (user, before, after) => Task.CompletedTask;

client.UserConnected += (user, channel, time) => Task.CompletedTask;
client.UserDisconnected += (user, channel, time) => Task.CompletedTask;
// client.LivestreamBegan += (user, channel) => Task.CompletedTask;
// client.LivestreamEnded += (user, channel) => Task.CompletedTask;

client.RoleCreated += role => Task.CompletedTask;
client.RoleDeleted += role => Task.CompletedTask;
client.RoleUpdated += (before, after) => Task.CompletedTask;

client.EmoteCreated += (emote, guild) => Task.CompletedTask;
client.EmoteDeleted += (emote, guild) => Task.CompletedTask;
client.EmoteUpdated += (before, after, guild) => Task.CompletedTask;

client.JoinedGuild += guild => Task.CompletedTask;
client.LeftGuild += guild => Task.CompletedTask;
client.GuildUpdated += (before, after) => Task.CompletedTask;
client.GuildAvailable += guild => Task.CompletedTask;
client.GuildUnavailable += guild => Task.CompletedTask;

client.MessageButtonClicked += MessageButtonClickedAsync;
client.DirectMessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;

#endregion

// 令牌（Tokens）应被视为机密数据，永远不应硬编码在代码中
// 在实际开发中，为了保护令牌的安全性，建议将令牌存储在安全的环境中
// 例如本地 .json、.yaml、.xml、.txt 文件、环境变量或密钥管理系统
// 这样可以避免将敏感信息直接暴露在代码中，以防止令牌被滥用或泄露
string token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)
    ?? throw new ArgumentNullException("KookDebugToken");

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

// 阻塞程序直到关闭
await Task.Delay(Timeout.Infinite);
return;

// Log 事件，此处以直接输出到控制台为例
Task LogAsync(LogMessage log)
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
}

// Ready 事件表示客户端已经建立了连接，现在可以安全地访问缓存
Task ReadyAsync()
{
    Console.WriteLine($"{client.CurrentUser} 已连接！");
    return Task.CompletedTask;
}

// 并不建议以这样的方式实现 Bot 的命令交互功能
// 请参阅 Kook.Net.Samples.TextCommands 示例项目及其文档
async Task MessageReceivedAsync(SocketMessage message,
    SocketGuildUser author,
    SocketTextChannel channel)
{
    // Bot 永远不应该响应自己的消息
    if (author.Id == client.CurrentUser?.Id)
        return;

    if (message.Content == "!ping")
    {
        // 创建一个 CardBuilder，卡片将会包含一个文本模块和一个按钮模块
        CardBuilder builder = new CardBuilder()
            .AddModule<SectionModuleBuilder>(s =>
                s.WithText("pong!"))
            .AddModule<ActionGroupModuleBuilder>(a => a
                .AddElement(b => b
                    .WithClick(ButtonClickEventType.ReturnValue)
                    .WithText("点我！")
                    .WithValue("unique-id")
                    .WithTheme(ButtonTheme.Primary)));

        // 发送一条卡片形式的消息，内容包含文本 pong!，以及一个按钮
        // 在调用时，需要先调用 .Build() 方法来构建卡片
        await channel.SendCardAsync(builder.Build());
    }
}

// 当按钮被点击时，会触发 MessageButtonClicked 事件
async Task MessageButtonClickedAsync(string value,
    Cacheable<SocketGuildUser, ulong> user,
    Cacheable<IMessage, Guid> message,
    SocketTextChannel channel)
{
    // 检查按钮的值是否为之前的代码中设置的值
    if (value == "unique-id")
    {
        IMessage? messageEntity = await message.GetOrDownloadAsync();
        if (messageEntity is IUserMessage userMessage)
            await userMessage.ReplyTextAsync("按钮被点击了！", isQuote: true);
    }

    else
        Console.WriteLine("接收到了一个没有对应处理程序的按钮值！");
}
