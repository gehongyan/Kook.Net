using Kook;
using Kook.WebSocket;

// 这是一个使用 Kook.Net 的 C# 简单示例

// 如果要编写带有命令的机器人，我们建议使用 Kook.Net.Commands 框架
// 而不是像在这个示例中一样自己处理它们。

// 您可以在以下位置找到使用命令框架的示例：
// - 这里可以看基于文本的命令框架的指南与示例
// - https://kooknet.dev/guides/text_commands/intro.html
internal class Program
{
    private readonly KookSocketClient _client;

    public static Task Main(string[] args) => new Program().MainAsync();

    public Program()
    {
        // KookSocketConfig 是 KookSocketClient 的配置类
        KookSocketConfig config = new()
        {
            AlwaysDownloadUsers = false,
            AlwaysDownloadVoiceStates = false,
            AlwaysDownloadBoostSubscriptions = false,
            MessageCacheSize = 100,
            LogLevel = LogSeverity.Debug
        };

        // 在使用完 Kook.Net 的客户端后，建议在应用程序的生命周期结束时进行 Dispose 操作
        _client = new KookSocketClient(config);

        // 此处列举了 Kook.Net 的 KookSocketClient 的所有事件

        #region BaseKookClient

        _client.Log += LogAsync;
        _client.LoggedIn += () => Task.CompletedTask;
        _client.LoggedOut += () => Task.CompletedTask;

        #endregion

        #region KookSocketClient

        _client.Connected += () => Task.CompletedTask;
        _client.Disconnected += exception => Task.CompletedTask;
        _client.Ready += ReadyAsync;
        _client.LatencyUpdated += (before, after) => Task.CompletedTask;

        #endregion

        #region BaseSocketClient

        _client.ChannelCreated += channel => Task.CompletedTask;
        _client.ChannelDestroyed += channel => Task.CompletedTask;
        _client.ChannelUpdated += (before, after) => Task.CompletedTask;

        _client.ReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
        _client.ReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;
        _client.DirectReactionAdded += (message, channel, user, reaction) => Task.CompletedTask;
        _client.DirectReactionRemoved += (message, channel, user, reaction) => Task.CompletedTask;

        _client.MessageReceived += MessageReceivedAsync;
        _client.MessageDeleted += (message, channel) => Task.CompletedTask;
        _client.MessageUpdated += (before, after, channel) => Task.CompletedTask;
        _client.MessagePinned += (before, after, channel, @operator) => Task.CompletedTask;
        _client.MessageUnpinned += (before, after, channel, @operator) => Task.CompletedTask;

        _client.DirectMessageReceived += (message, author, channel) => Task.CompletedTask;
        _client.DirectMessageDeleted += (message, author, channel) => Task.CompletedTask;
        _client.DirectMessageUpdated += (before, after, author, channel) => Task.CompletedTask;

        _client.UserJoined += (user, time) => Task.CompletedTask;
        _client.UserLeft += (guild, user, time) => Task.CompletedTask;
        _client.UserBanned += (users, @operator, guild, reason) => Task.CompletedTask;
        _client.UserUnbanned += (users, @operator, guild) => Task.CompletedTask;
        _client.UserUpdated += (before, after) => Task.CompletedTask;
        _client.CurrentUserUpdated += (before, after) => Task.CompletedTask;
        _client.GuildMemberUpdated += (before, after) => Task.CompletedTask;
        _client.GuildMemberOnline += (users, time) => Task.CompletedTask;
        _client.GuildMemberOffline += (users, time) => Task.CompletedTask;

        _client.UserConnected += (user, channel, time) => Task.CompletedTask;
        _client.UserDisconnected += (user, channel, time) => Task.CompletedTask;

        _client.RoleCreated += role => Task.CompletedTask;
        _client.RoleDeleted += role => Task.CompletedTask;
        _client.RoleUpdated += (before, after) => Task.CompletedTask;

        _client.EmoteCreated += (emote, guild) => Task.CompletedTask;
        _client.EmoteDeleted += (emote, guild) => Task.CompletedTask;
        _client.EmoteUpdated += (before, after, guild) => Task.CompletedTask;

        _client.JoinedGuild += guild => Task.CompletedTask;
        _client.LeftGuild += guild => Task.CompletedTask;
        _client.GuildUpdated += (before, after) => Task.CompletedTask;
        _client.GuildAvailable += guild => Task.CompletedTask;
        _client.GuildUnavailable += guild => Task.CompletedTask;

        _client.MessageButtonClicked += MessageButtonClickedAsync;
        _client.DirectMessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;

        #endregion
    }

    // Log 事件，此处以直接输出到控制台为例
    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    // Ready 事件表示客户端已经建立了连接，现在可以安全地访问缓存
    private Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} 已连接！");
        return Task.CompletedTask;
    }

    // 并不建议以这样的方式实现 Bot 的命令交互功能
    // 请参阅 Kook.Net.Samples.TextCommands 示例项目及其文档
    private async Task MessageReceivedAsync(SocketMessage message,
        SocketGuildUser author,
        SocketTextChannel channel)
    {
        // Bot 永远不应该响应自己的消息
        if (author.Id == _client.CurrentUser.Id)
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
    private async Task MessageButtonClickedAsync(string value,
        Cacheable<SocketGuildUser, ulong> user,
        Cacheable<IMessage, Guid> message,
        SocketTextChannel channel)
    {
        // 检查按钮的值是否为之前的代码中设置的值
        if (value == "unique-id")
        {
            IMessage messageEntity = await message.GetOrDownloadAsync();
            if (messageEntity is IUserMessage userMessage)
                await userMessage.ReplyTextAsync("按钮被点击了！", isQuote: true);
        }

        else
            Console.WriteLine("接收到了一个没有对应处理程序的按钮值！");
    }

    public async Task MainAsync()
    {
        // 令牌（Tokens）应被视为机密数据，永远不应硬编码在代码中
        // 在实际开发中，为了保护令牌的安全性，建议将令牌存储在安全的环境中
        // 例如本地 .json、.yaml、.xml、.txt 文件、环境变量或密钥管理系统
        // 这样可以避免将敏感信息直接暴露在代码中，以防止令牌被滥用或泄露
        string token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)
            ?? throw new ArgumentNullException("KookDebugToken");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // 阻塞程序直到关闭
        await Task.Delay(Timeout.Infinite);
    }
}
