// See https://aka.ms/new-console-template for more information

using Kook;
using Kook.Rest;
using Kook.WebSocket;

internal class Program
{
    private readonly KookSocketClient _client;
    private readonly string _token;
    public static Task Main(string[] args) => new Program().MainAsync();

    public Program()
    {
        _token = Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User)
            ?? throw new ArgumentNullException(nameof(_token));
        _client = new KookSocketClient(new KookSocketConfig
        {
            AlwaysDownloadUsers = false,
            AlwaysDownloadVoiceStates = false,
            AlwaysDownloadBoostSubscriptions = false,
            MessageCacheSize = 100,
            LogLevel = LogSeverity.Debug
        });

        #region BaseKookClient

        _client.Log += log => Task.Run(() => Console.WriteLine(log.ToString()));
        _client.LoggedIn += () => Task.CompletedTask;
        _client.LoggedOut += () => Task.CompletedTask;

        #endregion

        #region KookSocketClient

        _client.Connected += () => Task.CompletedTask;
        _client.Disconnected += exception => Task.CompletedTask;
        _client.Ready += async () =>
        {
            IReadOnlyCollection<RestFriendRequest> friendRequests = await _client.Rest.GetFriendRequestsAsync();
            IReadOnlyCollection<RestUser> friends = await _client.Rest.GetFriendsAsync();
            IReadOnlyCollection<RestUser> blockedUsers = await _client.Rest.GetBlockedUsersAsync();
        };
        _client.LatencyUpdated += (before, after) => Task.CompletedTask;

        #endregion

        #region BaseSocketClient

        _client.ChannelCreated += channel => Task.CompletedTask;
        _client.ChannelDestroyed += channel => Task.CompletedTask;
        _client.ChannelUpdated += (before, after) => Task.CompletedTask;

        _client.ReactionAdded += (message, channel, reaction) => Task.CompletedTask;
        _client.ReactionRemoved += (message, channel, reaction) => Task.CompletedTask;
        _client.DirectReactionAdded += (message, channel, reaction) => Task.CompletedTask;
        _client.DirectReactionRemoved += (message, channel, reaction) => Task.CompletedTask;

        _client.MessageReceived += message => Task.CompletedTask;
        _client.MessageDeleted += (message, channel) => Task.CompletedTask;
        _client.MessageUpdated += (before, after, channel) => Task.CompletedTask;
        _client.MessagePinned += (before, after, channel, @operator) => Task.CompletedTask;
        _client.MessageUnpinned += (before, after, channel, @operator) => Task.CompletedTask;

        _client.DirectMessageReceived += message => Task.CompletedTask;
        _client.DirectMessageDeleted += (message, channel) => Task.CompletedTask;
        _client.DirectMessageUpdated += (before, after, channel) => Task.CompletedTask;

        _client.UserJoined += (user, time) => Task.CompletedTask;
        _client.UserLeft += (guild, user, time) => Task.CompletedTask;
        _client.UserBanned += (users, @operator, guild) => Task.CompletedTask;
        _client.UserUnbanned += (users, @operator, guild) => Task.CompletedTask;
        _client.UserUpdated += (before, after) => Task.CompletedTask;
        _client.CurrentUserUpdated += (before, after) => Task.CompletedTask;
        _client.GuildMemberUpdated += (before, after) => Task.CompletedTask;
        _client.GuildMemberOnline += (users, time) => Task.CompletedTask;
        _client.GuildMemberOffline += (users, time) => Task.CompletedTask;

        _client.UserConnected += (user, channel, guild, time) => Task.CompletedTask;
        _client.UserDisconnected += (user, channel, guild, time) => Task.CompletedTask;

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

        _client.MessageButtonClicked += (value, user, message, channel, guild) => Task.CompletedTask;
        _client.DirectMessageButtonClicked += (value, user, message, channel) => Task.CompletedTask;

        #endregion
    }

    public async Task MainAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }
}
