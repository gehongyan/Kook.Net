using System;
using System.Threading.Tasks;
using Kook.WebSocket;
using Xunit;

namespace Kook;

[CollectionDefinition(nameof(MessageTests), DisableParallelization = true)]
[Trait("Category", "Integration.Socket")]
public class ReactionTests : IClassFixture<SocketChannelFixture>, IClassFixture<ReactionTestFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly KookSocketClient _client;
    private readonly SocketTextChannel _textChannel;

    public ReactionTests(SocketChannelFixture channelFixture, ReactionTestFixture reactionFixture, ITestOutputHelper output)
    {
        _output = output;
        _textChannel = channelFixture.TextChannel;
        _client = channelFixture.Client;
        reactionFixture.EnsureLogger(_client, LogAsync);
    }

    private Task LogAsync(LogMessage message)
    {
        _output.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddRemoveEmojiReaction()
    {
        Assert.NotNull(_client.CurrentUser);

        // Send a text message
        const string content = "TEXT CONTENT";
        Cacheable<IUserMessage, Guid> cacheableUserMessage = await _textChannel.SendTextAsync(content);
        IUserMessage? userMessage = await cacheableUserMessage.GetOrDownloadAsync();
        Assert.NotNull(userMessage);
        Guid messageId = cacheableUserMessage.Id;

        // Add a reaction to the message
        Emoji emoji = new("👍");
        TaskCompletionSource<IMessage> messagePromise = new();
        TaskCompletionSource<SocketTextChannel> channelPromise = new();
        TaskCompletionSource<SocketGuildUser> operatorPromise = new();
        TaskCompletionSource<SocketReaction> reactionPromise = new();
        IMessage? message = null;
        SocketTextChannel? channel = null;
        SocketGuildUser? operatorUser = null;
        SocketReaction? reaction = null;
        _client.ReactionAdded += ReactionEventHandler;

        // The reaction and its related message, channel, and operator should match
        await userMessage.AddReactionAsync(emoji);
        await WaitForPromises();
        _client.ReactionAdded -= ReactionEventHandler;
        AssertValues();

        // Reset values for removing assertions
        messagePromise = new TaskCompletionSource<IMessage>();
        channelPromise = new TaskCompletionSource<SocketTextChannel>();
        operatorPromise = new TaskCompletionSource<SocketGuildUser>();
        reactionPromise = new TaskCompletionSource<SocketReaction>();
        message = null;
        channel = null;
        operatorUser = null;
        reaction = null;
        _client.ReactionRemoved += ReactionEventHandler;

        // The reaction and its related message, channel, and operator should match
        await userMessage.RemoveReactionAsync(emoji, _client.CurrentUser);
        await WaitForPromises();
        _client.ReactionRemoved -= ReactionEventHandler;
        AssertValues();

        return;

        // ReSharper disable AccessToModifiedClosure
        async Task ReactionEventHandler(Cacheable<IMessage, Guid> cacheableMessage,
            SocketTextChannel socketTextChannel,
            Cacheable<SocketGuildUser, ulong> cacheableOperator,
            SocketReaction socketReaction)
        {
            IMessage? downloadedMessage = await cacheableMessage.GetOrDownloadAsync();
            Assert.NotNull(downloadedMessage);
            messagePromise.SetResult(downloadedMessage);
            channelPromise.SetResult(socketTextChannel);
            SocketGuildUser? downloadedUser = await cacheableOperator.GetOrDownloadAsync();
            Assert.NotNull(downloadedUser);
            operatorPromise.SetResult(downloadedUser);
            reactionPromise.SetResult(socketReaction);
        }

        async Task WaitForPromises()
        {
            message = await messagePromise.Task;
            channel = await channelPromise.Task;
            operatorUser = await operatorPromise.Task;
            reaction = await reactionPromise.Task;
        }
        // ReSharper restore AccessToModifiedClosure

        void AssertValues()
        {
            Assert.NotNull(message);
            Assert.NotNull(channel);
            Assert.NotNull(operatorUser);
            Assert.NotNull(reaction);
            Assert.Equal(messageId, message.Id);
            Assert.Same(_textChannel, channel);
            Assert.NotNull(_client.CurrentUser);
            Assert.Equal(_client.CurrentUser.Id, operatorUser.Id);
            Assert.Equal("👍", reaction.Emote.Name);
            Assert.Equal("👍", reaction.Emote.Id);
            Assert.Equal(operatorUser.Id, reaction.UserId);
            Assert.Same(operatorUser, reaction.User);
            Assert.Equal(messageId, reaction.MessageId);
            Assert.Same(message, reaction.Message);
            Assert.NotNull(reaction.Channel);
            Assert.Equal(channel.Id, reaction.Channel.Id);
            Assert.Same(channel, reaction.Channel);
        }
    }
}

public class ReactionTestFixture : IAsyncDisposable
{
    private Action? _loggerDisposer;

    private bool _loggerSubscribed;

    public void EnsureLogger(KookSocketClient client, Func<LogMessage, Task> logAction)
    {
        if (_loggerSubscribed) return;
        client.Log += logAction;
        _loggerDisposer += () => client.Log -= logAction;
        _loggerSubscribed = true;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _loggerDisposer?.Invoke();
        return ValueTask.CompletedTask;
    }
}
