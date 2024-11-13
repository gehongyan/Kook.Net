open System
open System.Threading
open System.Threading.Tasks
open Kook
open Kook.WebSocket

// 这是一个使用 Kook.Net 的 F# 简单示例
// Kook.Net 的所有文档均以 C# 编写，但是 Kook.Net 也支持 F#

// 如果要编写带有命令的 Bot，我们建议使用 Kook.Net.Commands 框架
// 而不是像在这个示例中一样自己处理它们。

// 您可以在以下位置找到使用命令框架的示例：
// - 这里可以看基于文本的命令框架的指南与示例
// - https://kooknet.dev/guides/text_commands/intro.html

// KookSocketConfig 是 KookSocketClient 的配置类
let config =
    KookSocketConfig(
        AlwaysDownloadUsers = false,
        AlwaysDownloadVoiceStates = false,
        AlwaysDownloadBoostSubscriptions = false,
        MessageCacheSize = 100,
        LogLevel = LogSeverity.Debug
    )

// 在使用完 Kook.Net 的客户端后，建议在应用程序的生命周期结束时进行 Dispose 操作
// 由于 F# 中 use 绑定在模块中被视为 let 绑定，此处使用 let 来创建一个作用域
let client = new KookSocketClient(config)

// Log 事件，此处以直接输出到控制台为例
let LogAsync (log: LogMessage) : Task =
    Console.WriteLine(log.ToString())
    Task.CompletedTask

// Ready 事件表示客户端已经建立了连接，现在可以安全地访问缓存
let ReadyAsync () : Task =
    Console.WriteLine($"{client.CurrentUser} is connected!")
    Task.CompletedTask

// 并不建议以这样的方式实现 Bot 的命令交互功能
// 请参阅 Kook.Net.Samples.TextCommands 示例项目及其文档
let MessageReceivedAsync (message: SocketMessage) (author: SocketGuildUser) (channel: SocketTextChannel) : Task =
    task {
        let currentUserId: Nullable<uint64> =
            match client.CurrentUser with
            | null -> Nullable()
            | s -> Nullable s.Id

        if currentUserId.HasValue && author.Id = currentUserId.Value then
            ()

        if message.Content = "!ping" then
            // 创建一个 CardBuilder，卡片将会包含一个文本模块和一个按钮模块
            let builder =
                CardBuilder()
                    .AddModule<SectionModuleBuilder>(fun s -> s.WithText("Pong!") |> ignore)
                    .AddModule<ActionGroupModuleBuilder>(fun a ->
                        a.AddElement(fun b ->
                            b
                                .WithClick(ButtonClickEventType.ReturnValue)
                                .WithText("点我！")
                                .WithValue("unique-id")
                                .WithTheme(ButtonTheme.Primary)
                            |> ignore)
                        |> ignore)

            // 发送一条卡片形式的消息，内容包含文本 pong!，以及一个按钮
            // 在调用时，需要先调用 .Build() 方法来构建卡片
            do! channel.SendCardAsync(builder.Build()) |> Async.AwaitTask |> Async.Ignore
    }

// 当按钮被点击时，会触发 MessageButtonClicked 事件
let MessageButtonClickedAsync
    (value: string)
    (user: Cacheable<SocketGuildUser, uint64>)
    (message: Cacheable<IMessage, Guid>)
    (channel: SocketTextChannel)
    : Task =
    task {
        // 检查按钮的值是否为之前的代码中设置的值
        if value = "unique-id" then
            let! messageEntity = message.GetOrDownloadAsync()

            match messageEntity with
            | :? IUserMessage as userMessage ->
                // 回复消息
                do!
                    userMessage.ReplyTextAsync("按钮被点击了！", isQuote = true)
                    |> Async.AwaitTask
                    |> Async.Ignore
            | _ -> ()
        else
            Console.WriteLine("接收到了一个没有对应处理程序的按钮值！")
    }

// 此处列举了 Kook.Net 的 KookSocketClient 的所有事件

// BaseKookClient

client.add_Log LogAsync
client.add_LoggedIn (fun () -> Task.CompletedTask)
client.add_LoggedOut (fun () -> Task.CompletedTask)

// KookSocketClient

client.add_Connected (fun () -> Task.CompletedTask)
client.add_Disconnected (fun ``exception`` -> Task.CompletedTask)
client.add_Ready ReadyAsync
client.add_LatencyUpdated (fun before after -> Task.CompletedTask)

// BaseSocketClient

client.add_ChannelCreated (fun channel -> Task.CompletedTask)
client.add_ChannelDestroyed (fun channel -> Task.CompletedTask)
client.add_ChannelUpdated (fun before after -> Task.CompletedTask)

client.add_ReactionAdded (fun message channel user reaction -> Task.CompletedTask)
client.add_ReactionRemoved (fun message channel user reaction -> Task.CompletedTask)
client.add_DirectReactionAdded (fun message channel user reaction -> Task.CompletedTask)
client.add_DirectReactionRemoved (fun message channel user reaction -> Task.CompletedTask)
client.add_MessageReceived MessageReceivedAsync
client.add_MessageDeleted (fun message channel -> Task.CompletedTask)
client.add_MessageUpdated (fun before after channel -> Task.CompletedTask)
client.add_MessagePinned (fun before after channel operator -> Task.CompletedTask)
client.add_MessageUnpinned (fun before after channel operator -> Task.CompletedTask)

client.add_DirectMessageReceived (fun message author channel -> Task.CompletedTask)
client.add_DirectMessageDeleted (fun message author channel -> Task.CompletedTask)
client.add_DirectMessageUpdated (fun before after author channel -> Task.CompletedTask)

client.add_UserJoined (fun user time -> Task.CompletedTask)
client.add_UserLeft (fun guild user time -> Task.CompletedTask)
client.add_UserBanned (fun users operator guild reason -> Task.CompletedTask)
client.add_UserUnbanned (fun users operator guild -> Task.CompletedTask)
client.add_UserUpdated (fun before after -> Task.CompletedTask)
client.add_CurrentUserUpdated (fun before after -> Task.CompletedTask)
client.add_GuildMemberUpdated (fun before after -> Task.CompletedTask)
client.add_GuildMemberOnline (fun users time -> Task.CompletedTask)
client.add_GuildMemberOffline (fun users time -> Task.CompletedTask)
client.add_UserVoiceStateUpdated (fun user before after -> Task.CompletedTask)

client.add_UserConnected (fun user channel time -> Task.CompletedTask)
client.add_UserDisconnected (fun user channel time -> Task.CompletedTask)
// client.add_LivestreamBegan (fun user channel -> Task.CompletedTask)
// client.add_LivestreamEnded (fun user channel -> Task.CompletedTask)

client.add_RoleCreated (fun role -> Task.CompletedTask)
client.add_RoleDeleted (fun role -> Task.CompletedTask)
client.add_RoleUpdated (fun before after -> Task.CompletedTask)

client.add_EmoteCreated (fun emote guild -> Task.CompletedTask)
client.add_EmoteDeleted (fun emote guild -> Task.CompletedTask)
client.add_EmoteUpdated (fun before after guild -> Task.CompletedTask)

client.add_JoinedGuild (fun guild -> Task.CompletedTask)
client.add_LeftGuild (fun guild -> Task.CompletedTask)
client.add_GuildUpdated (fun before after -> Task.CompletedTask)
client.add_GuildAvailable (fun guild -> Task.CompletedTask)
client.add_GuildUnavailable (fun guild -> Task.CompletedTask)

client.add_MessageButtonClicked MessageButtonClickedAsync
client.add_DirectMessageButtonClicked (fun value user message channel -> Task.CompletedTask)

// 令牌（Tokens）应被视为机密数据，永远不应硬编码在代码中
// 在实际开发中，为了保护令牌的安全性，建议将令牌存储在安全的环境中
// 例如本地 .json、.yaml、.xml、.txt 文件、环境变量或密钥管理系统
// 这样可以避免将敏感信息直接暴露在代码中，以防止令牌被滥用或泄露
let token =
    match Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User) with
    | null -> raise (ArgumentNullException("KookDebugToken"))
    | x -> x

// 阻塞程序直到关闭
async {
    do! client.LoginAsync(TokenType.Bot, token) |> Async.AwaitTask
    do! client.StartAsync() |> Async.AwaitTask

    // 阻塞程序直到关闭
    do! Task.Delay(Timeout.Infinite) |> Async.AwaitTask
    client.Dispose()
}
|> Async.RunSynchronously
