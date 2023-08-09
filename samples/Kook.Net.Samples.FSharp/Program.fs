open System
open System.Threading
open System.Threading.Tasks
open Kook
open Kook.WebSocket

let token =
    Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)

if token = null then
    raise (ArgumentNullException(nameof token))

let config =
    KookSocketConfig(
        AlwaysDownloadUsers = false,
        AlwaysDownloadVoiceStates = false,
        AlwaysDownloadBoostSubscriptions = false,
        MessageCacheSize = 100,
        LogLevel = LogSeverity.Debug
    )

let client = new KookSocketClient(config)

// BaseKookClient

client.add_Log (fun log -> Task.Run(fun () -> Console.WriteLine(log.ToString())))
client.add_LoggedIn (fun () -> Task.CompletedTask)
client.add_LoggedOut (fun () -> Task.CompletedTask)

// KookSocketClient

client.add_Connected (fun () -> Task.CompletedTask)
client.add_Disconnected (fun ``exception`` -> Task.CompletedTask)
client.add_Ready (fun () -> Task.CompletedTask)
client.add_LatencyUpdated (fun before after -> Task.CompletedTask)

// BaseSocketClient

client.add_ChannelCreated (fun channel -> Task.CompletedTask)
client.add_ChannelDestroyed (fun channel -> Task.CompletedTask)
client.add_ChannelUpdated (fun before after -> Task.CompletedTask)

client.add_ReactionAdded (fun message channel user reaction -> Task.CompletedTask)
client.add_ReactionRemoved (fun message channel user reaction -> Task.CompletedTask)
client.add_DirectReactionAdded (fun message channel user reaction -> Task.CompletedTask)
client.add_DirectReactionRemoved (fun message channel user reaction -> Task.CompletedTask)

client.add_MessageReceived (fun message author channel -> Task.CompletedTask)
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

client.add_UserConnected (fun user channel time -> Task.CompletedTask)
client.add_UserDisconnected (fun user channel time -> Task.CompletedTask)

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

client.add_MessageButtonClicked (fun value user message channel -> Task.CompletedTask)
client.add_DirectMessageButtonClicked (fun value user message channel -> Task.CompletedTask)

async {
    do! client.LoginAsync(TokenType.Bot, token) |> Async.AwaitTask
    do! client.StartAsync() |> Async.AwaitTask
    do! Task.Delay(Timeout.Infinite) |> Async.AwaitTask
}
|> Async.RunSynchronously
