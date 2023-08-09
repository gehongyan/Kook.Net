Imports System
Imports System.Threading
Imports Kook.WebSocket

Public Class Program
    Private ReadOnly _client As KookSocketClient
    Private ReadOnly _token As String

    Public Shared Sub Main(args As String())
        Dim program As New Program()
        program.MainAsync().GetAwaiter().GetResult()
    End Sub

    Public Sub New()
        _token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)
        If _token Is Nothing Then
            Throw New ArgumentNullException(NameOf(_token))
        End If
        Dim config As New KookSocketConfig() With {
                .AlwaysDownloadUsers = False,
                .AlwaysDownloadVoiceStates = False,
                .AlwaysDownloadBoostSubscriptions = False,
                .MessageCacheSize = 100,
                .LogLevel = LogSeverity.Debug
                }

        _client = New KookSocketClient(config)

#Region "Event Handlers"

        AddHandler _client.Log, Function(log)
            Return Task.Run(Sub()
                Console.WriteLine(log.ToString())
            End Sub)
        End Function

        AddHandler _client.LoggedIn, Function()
            Return Task.CompletedTask
        End Function

        AddHandler _client.LoggedOut, Function()
            Return Task.CompletedTask
        End Function

#End Region

#Region "KookSocketClient"

        AddHandler _client.Connected, Function()
            Return Task.CompletedTask
        End Function

        AddHandler _client.Disconnected, Function(exception)
            Return Task.CompletedTask
        End Function

        AddHandler _client.Ready, Function()
            Return Task.CompletedTask
        End Function

        AddHandler _client.LatencyUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function

#End Region

#Region "BaseSocketClient"

        AddHandler _client.ChannelCreated, Function(channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.ChannelDestroyed, Function(channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.ChannelUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function

        AddHandler _client.ReactionAdded, Function(message, channel, user, reaction)
            Return Task.CompletedTask
        End Function
        AddHandler _client.ReactionRemoved, Function(message, channel, user, reaction)
            Return Task.CompletedTask
        End Function
        AddHandler _client.DirectReactionAdded, Function(message, channel, user, reaction)
            Return Task.CompletedTask
        End Function
        AddHandler _client.DirectReactionRemoved, Function(message, channel, user, reaction)
            Return Task.CompletedTask
        End Function

        AddHandler _client.MessageReceived, Function(message, author, channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.MessageDeleted, Function(message, channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.MessageUpdated, Function(before, after, channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.MessagePinned, Function(before, after, channel, [operator])
            Return Task.CompletedTask
        End Function
        AddHandler _client.MessageUnpinned, Function(before, after, channel, [operator])
            Return Task.CompletedTask
        End Function

        AddHandler _client.DirectMessageReceived, Function(message, author, channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.DirectMessageDeleted, Function(message, author, channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.DirectMessageUpdated, Function(before, after, author, channel)
            Return Task.CompletedTask
        End Function

        AddHandler _client.UserJoined, Function(user, time)
            Return Task.CompletedTask
        End Function
        AddHandler _client.UserLeft, Function(guild, user, time)
            Return Task.CompletedTask
        End Function
        AddHandler _client.UserBanned, Function(users, [operator], guild, reason)
            Return Task.CompletedTask
        End Function
        AddHandler _client.UserUnbanned, Function(users, [operator], guild)
            Return Task.CompletedTask
        End Function
        AddHandler _client.UserUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function
        AddHandler _client.CurrentUserUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function
        AddHandler _client.GuildMemberUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function
        AddHandler _client.GuildMemberOnline, Function(users, time)
            Return Task.CompletedTask
        End Function
        AddHandler _client.GuildMemberOffline, Function(users, time)
            Return Task.CompletedTask
        End Function

        AddHandler _client.UserConnected, Function(user, channel, time)
            Return Task.CompletedTask
        End Function
        AddHandler _client.UserDisconnected, Function(user, channel, time)
            Return Task.CompletedTask
        End Function

        AddHandler _client.RoleCreated, Function(role)
            Return Task.CompletedTask
        End Function
        AddHandler _client.RoleDeleted, Function(role)
            Return Task.CompletedTask
        End Function
        AddHandler _client.RoleUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function

        AddHandler _client.EmoteCreated, Function(emote, guild)
            Return Task.CompletedTask
        End Function
        AddHandler _client.EmoteDeleted, Function(emote, guild)
            Return Task.CompletedTask
        End Function
        AddHandler _client.EmoteUpdated, Function(before, after, guild)
            Return Task.CompletedTask
        End Function

        AddHandler _client.JoinedGuild, Function(guild)
            Return Task.CompletedTask
        End Function
        AddHandler _client.LeftGuild, Function(guild)
            Return Task.CompletedTask
        End Function
        AddHandler _client.GuildUpdated, Function(before, after)
            Return Task.CompletedTask
        End Function
        AddHandler _client.GuildAvailable, Function(guild)
            Return Task.CompletedTask
        End Function
        AddHandler _client.GuildUnavailable, Function(guild)
            Return Task.CompletedTask
        End Function

        AddHandler _client.MessageButtonClicked, Function(value, user, message, channel)
            Return Task.CompletedTask
        End Function
        AddHandler _client.DirectMessageButtonClicked, Function(value, user, message, channel)
            Return Task.CompletedTask
        End Function

#End Region
    End Sub

    Public Async Function MainAsync() As Task
        Await _client.LoginAsync(TokenType.Bot, _token)
        Await _client.StartAsync()
        Await Task.Delay(Timeout.Infinite)
    End Function
End Class
