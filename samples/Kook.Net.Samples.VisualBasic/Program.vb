Imports System.Threading
Imports Kook.WebSocket

' 这是一个使用 Kook.Net 的 Visual Basic 简单示例
' Kook.Net 的所有文档均以 C# 编写，但是 Kook.Net 也支持 Visual Basic

' 如果要编写带有命令的机器人，我们建议使用 Kook.Net.Commands 框架
' 而不是像在这个示例中一样自己处理它们。

' 您可以在以下位置找到使用命令框架的示例：
' - 这里可以看基于文本的命令框架的指南与示例
' - https://kooknet.dev/guides/text_commands/intro.html
Public Class Program
    Private ReadOnly _client As KookSocketClient

    Public Shared Sub Main(args As String())
        Dim program As New Program()
        program.MainAsync().GetAwaiter().GetResult()
    End Sub

    Public Async Function MainAsync() As Task
        ' 令牌（Tokens）应被视为机密数据，永远不应硬编码在代码中
        ' 在实际开发中，为了保护令牌的安全性，建议将令牌存储在安全的环境中
        ' 例如本地 .json、.yaml、.xml、.txt 文件、环境变量或密钥管理系统
        ' 这样可以避免将敏感信息直接暴露在代码中，以防止令牌被滥用或泄露
        Dim token As String = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)
        If token Is Nothing Then
            Throw New ArgumentNullException("KookDebugToken")
        End If

        Await _client.LoginAsync(TokenType.Bot, token)
        Await _client.StartAsync()

        ' 阻塞程序直到关闭
        Await Task.Delay(Timeout.Infinite)
        _client.Dispose()
    End Function

    Public Sub New()
        ' KookSocketConfig 是 KookSocketClient 的配置类
        Dim config As New KookSocketConfig() With {
                .AlwaysDownloadUsers = False,
                .AlwaysDownloadVoiceStates = False,
                .AlwaysDownloadBoostSubscriptions = False,
                .MessageCacheSize = 100,
                .LogLevel = LogSeverity.Debug
                }

        ' 在使用完 Kook.Net 的客户端后，建议在应用程序的生命周期结束时进行 Dispose 操作
        _client = New KookSocketClient(config)

        ' 此处列举了 Kook.Net 的 KookSocketClient 的所有事件
#Region "Event Handlers"

        AddHandler _client.Log, AddressOf LogAsync

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

        AddHandler _client.Ready, AddressOf ReadyAsync

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

        AddHandler _client.MessageReceived, AddressOf MessageReceivedAsync
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
        AddHandler _client.UserVoiceStateUpdated, Function(user, before, after)
            Return Task.CompletedTask
        End Function

        AddHandler _client.UserConnected, Function(user, channel, time)
            Return Task.CompletedTask
        End Function
        AddHandler _client.UserDisconnected, Function(user, channel, time)
            Return Task.CompletedTask
        End Function
'        AddHandler _client.LivestreamBegan, Function(user, channel)
'            Return Task.CompletedTask
'        End Function
'        AddHandler _client.LivestreamEnded, Function(user, channel)
'            Return Task.CompletedTask
'        End Function

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

        AddHandler _client.MessageButtonClicked, AddressOf MessageButtonClickedAsync
        AddHandler _client.DirectMessageButtonClicked, Function(value, user, message, channel)
            Return Task.CompletedTask
        End Function

#End Region
    End Sub

    ' Log 事件，此处以直接输出到控制台为例

    Public Shared Function LogAsync(log As LogMessage) As Task
        Console.WriteLine(log.ToString())
        Return Task.CompletedTask
    End Function

    ' Ready 事件表示客户端已经建立了连接，现在可以安全地访问缓存

    Public Function ReadyAsync() As Task
        Console.WriteLine($"{_client.CurrentUser} 已连接！")
        Return Task.CompletedTask
    End Function

    Public Async Function MessageReceivedAsync(message As SocketMessage,
                                               author As SocketGuildUser,
                                               channel As SocketTextChannel) As Task
        ' Bot 永远不应该响应自己的消息
        If author.Id = _client.CurrentUser.Id Then
            Return
        End If

        If message.Content = "!ping" Then
            ' 创建一个 CardBuilder，卡片将会包含一个文本模块和一个按钮模块
            Dim builder As New CardBuilder()
            builder.AddModule (Of SectionModuleBuilder)(Sub(s)
                s.WithText("pong!")
            End Sub)
            builder.AddModule (Of ActionGroupModuleBuilder)(Sub(a)
                a.AddElement(Sub(b)
                    b.WithClick(ButtonClickEventType.ReturnValue)
                    b.WithText("点我！")
                    b.WithValue("unique-id")
                    b.WithTheme(ButtonTheme.Primary)
                End Sub)
            End Sub)

            ' 发送一条卡片形式的消息，内容包含文本 pong!，以及一个按钮
            ' 在调用时，需要先调用 .Build() 方法来构建卡片
            Await channel.SendCardAsync(builder.Build())
        End If
    End Function

    Public Async Function MessageButtonClickedAsync(value As String,
                                                    user As Cacheable(Of SocketGuildUser, ULong),
                                                    message As Cacheable(Of IMessage, Guid),
                                                    channel As SocketTextChannel) As Task
        ' 检查按钮的值是否为之前的代码中设置的值
        If value = "unique-id" Then
            Dim messageEntity As IMessage = Await message.GetOrDownloadAsync()
            Dim userMessage = TryCast(messageEntity, IUserMessage)
            If userMessage IsNot Nothing Then
                Await userMessage.ReplyTextAsync("按钮被点击了！", isQuote := true)
            End If
        Else
            Console.WriteLine("接收到了一个没有对应处理程序的按钮值！")
        End If
    End Function
End Class
